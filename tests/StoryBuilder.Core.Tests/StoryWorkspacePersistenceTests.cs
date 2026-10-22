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
        var character = session.CreateCharacter("Mara", "Investigator")
            .WithDescription("A brave explorer.")
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
        Assert.Equal(character.Id, Assert.Single(restoredSession.Workspace.Characters).Id);
        Assert.Equal(scene.Id, Assert.Single(restoredSession.Workspace.Scenes).Id);
        Assert.Equal("left hand on the latch", Assert.Single(restoredSession.Workspace.Scenes[0].Beats).PoseOrPosition);
        Assert.Equal(source.Id, Assert.Single(restoredSession.Workspace.Sources).Id);
        Assert.Equal(campaign.Id, Assert.Single(restoredSession.Workspace.Campaigns).Id);
        Assert.Equal(CanonStatus.Proposed, Assert.Single(restoredSession.Workspace.ParticipantActions).Status);
        Assert.Equal("courageous", Assert.Single(restoredSession.Vocabulary.Search("brave")).CanonicalTerm);
        Assert.Contains(restoredSession.Vocabulary.Search("brave"), usage => usage.OccurrenceCount >= 1);
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
