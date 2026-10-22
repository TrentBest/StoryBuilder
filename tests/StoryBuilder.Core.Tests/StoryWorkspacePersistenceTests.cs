using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryWorkspacePersistenceTests
{
    [Fact]
    public void BinaryDocumentRoundTripsWorkspaceRecordsAndSynonymMappings()
    {
        var session = new StoryBuilderSession("Persistence campaign");
        var premise = session.CreatePremise(
            "A starship cartographer returns to a vanished Earth.",
            title: "The Lost Cartographer",
            scale: "Connected series",
            genre: "Science fiction",
            primaryTarget: "Novel");
        var character = session.CreateCharacter("Mara", "Investigator") with { NarrativeRole = CharacterNarrativeRole.Hero };
        character = character.WithDescription("A brave explorer.")
            .AddTrait(new CharacterTrait { Term = "methodical", Attribution = "Author" });
        session.UpdateCharacter(character);
        session.AddSynonym("courageous", "brave");

        var scene = session.CreateScene("North Gate", "Old wall", SceneCaptureMode.Claymation)
            .AddBeat(new SceneBeat
            {
                Kind = SceneBeatKind.Pose,
                ActorId = character.Id,
                ActorLabel = character.Name,
                PoseOrPosition = "left hand on the latch",
                Attribution = "Participant",
                Status = CanonStatus.Proposed
            });
        session.UpdateScene(scene);

        var source = session.AddSource(new SourceRecord
        {
            Title = "Recorder transcript",
            SourceType = "Artifact",
            Perspective = "Recovered from the north gate"
        });
        session.AddHistoricalClaim(new HistoricalClaim
        {
            Statement = "The gate was sealed.",
            InWorldDateOrPeriod = "Late Expansion Age",
            SourceIds = [source.Id],
            Status = CanonStatus.Accepted
        });

        var campaign = session.AddCampaign(new CampaignRecord { Name = "Lost Cartographers" });
        session.AddParticipantAction(new ParticipantAction
        {
            CampaignId = campaign.Id,
            CharacterId = character.Id,
            Description = "We open the gate.",
            Attribution = "Player",
            Status = CanonStatus.Proposed
        });

        var document = StoryWorkspaceBinaryDocument.FromSession(session);
        using var output = new MemoryBinaryStream();
        document.Pack(output);
        var payload = output.ToArray();

        using var input = new MemoryBinaryStream(payload);
        var restoredDocument = new StoryWorkspaceBinaryDocument();
        restoredDocument.Unpack(input);
        var restoredSession = new StoryBuilderSession("temporary");
        restoredDocument.RestoreInto(restoredSession);

        Assert.Equal(session.Workspace.Id, restoredSession.Workspace.Id);
        Assert.Equal("Persistence campaign", restoredSession.Workspace.Name);
        var restoredPremise = Assert.Single(restoredSession.Workspace.Premises);
        Assert.Equal(premise.Id, restoredPremise.Id);
        Assert.Equal("The Lost Cartographer", restoredPremise.Title);
        Assert.Equal("Connected series", restoredPremise.Scale);
        var restoredCharacter = Assert.Single(restoredSession.Workspace.Characters);
        Assert.Equal(character.Id, restoredCharacter.Id);
        Assert.Equal(CharacterNarrativeRole.Hero, restoredCharacter.NarrativeRole);
        Assert.Equal(scene.Id, Assert.Single(restoredSession.Workspace.Scenes).Id);
        Assert.Equal("left hand on the latch", Assert.Single(restoredSession.Workspace.Scenes[0].Beats).PoseOrPosition);
        Assert.Equal(source.Id, Assert.Single(restoredSession.Workspace.Sources).Id);
        Assert.Equal(campaign.Id, Assert.Single(restoredSession.Workspace.Campaigns).Id);
        Assert.Equal(CanonStatus.Proposed, Assert.Single(restoredSession.Workspace.ParticipantActions).Status);
        Assert.Equal("courageous", Assert.Single(restoredSession.Vocabulary.Search("brave")).CanonicalTerm);
        Assert.Contains(restoredSession.Vocabulary.Search("brave"), usage => usage.OccurrenceCount >= 1);
    }

    [Fact]
    public void VersionOneWorkspacePayloadMigratesWithUndecidedCharacterRolesAndNoPremises()
    {
        using var input = new MemoryBinaryStream(CreateVersionOneEmptyWorkspacePayload());
        var document = new StoryWorkspaceBinaryDocument();

        document.Unpack(input);

        Assert.Equal("Legacy workspace", document.Workspace.Name);
        Assert.Empty(document.Workspace.Premises);
        Assert.Empty(document.Workspace.Characters);
    }

    [Fact]
    public void BinaryDocumentRejectsUnknownFormatVersion()
    {
        var session = new StoryBuilderSession("Version test");
        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        var payload = output.ToArray();
        payload[4] = 99;

        using var input = new MemoryBinaryStream(payload);
        var document = new StoryWorkspaceBinaryDocument();
        Assert.Throws<InvalidDataException>(() => document.Unpack(input));
    }

    private static byte[] CreateVersionOneEmptyWorkspacePayload()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("SBWS"));
        writer.Write(1);
        writer.Write(Guid.Empty.ToByteArray());
        WriteLegacyString(writer, "Legacy workspace");
        writer.Write(Guid.Empty.ToByteArray());
        WriteLegacyString(writer, "Legacy experience");
        WriteLegacyString(writer, "Legacy purpose");
        WriteLegacyString(writer, "Legacy audience");
        for (var i = 0; i < 12; i++)
            writer.Write(0);
        writer.Flush();
        return stream.ToArray();
    }

    private static void WriteLegacyString(BinaryWriter writer, string? value)
    {
        if (value is null)
        {
            writer.Write(-1);
            return;
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }

    [Fact]
    public void BinaryDocumentRejectsTruncatedPayload()
    {
        var session = new StoryBuilderSession("Truncation test");
        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        var payload = output.ToArray()[..^1];

        using var input = new MemoryBinaryStream(payload);
        var document = new StoryWorkspaceBinaryDocument();
        Assert.ThrowsAny<IOException>(() => document.Unpack(input));
    }
}
