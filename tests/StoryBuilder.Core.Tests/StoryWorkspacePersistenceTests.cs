using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryWorkspacePersistenceTests
{
    [Fact]
    public void LegacyImportPreservesOriginalJsonAcrossBinarySaveAndRestore()
    {
        const string legacyJson = """
        {
          "schemaVersion": 1,
          "projectId": "8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb1",
          "title": "Offset Test",
          "premise": "A promise is broken.",
          "manuscript": "Line one.\r\nLine two.",
          "createdUtc": "2024-01-02T03:04:05-08:00",
          "updatedUtc": "2024-02-03T04:05:06+05:30",
          "passages": [
            { "id": "8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb2", "text": "Line two." }
          ],
          "timeline": [
            {
              "id": "8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb3",
              "title": "The promise",
              "description": "It is broken.",
              "occurredAt": "2025-06-07T08:09:10+05:30",
              "passageId": "8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb2",
              "observerId": "witness-17"
            }
          ]
        }
        """;

        var workspace = LegacyStoryProjectImporter.Import(legacyJson);

        Assert.Equal(Guid.Parse("8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb1"), workspace.Id);
        Assert.Equal(legacyJson, workspace.LegacyProjectSourceJson);
        Assert.Equal("A promise is broken.", Assert.Single(workspace.Premises).Vision);
        Assert.Equal("Line one.\r\nLine two.", Assert.Single(Assert.Single(workspace.Manuscripts).Sections).Body);
        var importedEvent = Assert.Single(workspace.Events);
        Assert.Equal(Guid.Parse("8b3b46e8-1d37-4a92-a2ce-2d08b37d4bb3"), importedEvent.Id);
        Assert.NotNull(importedEvent.InWorldDateOrPeriod);
        Assert.NotNull(importedEvent.InWorldDateOrPeriod);
        Assert.StartsWith("2025-06-07T08:09:10", importedEvent.InWorldDateOrPeriod);
        Assert.EndsWith("+05:30", importedEvent.InWorldDateOrPeriod);
        Assert.Contains("Line two.", importedEvent.Summary);

        var document = new StoryWorkspaceBinaryDocument(workspace);
        using var output = new MemoryBinaryStream();
        document.Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        Assert.Equal(legacyJson, restored.Workspace.LegacyProjectSourceJson);
        Assert.Equal(importedEvent.Id, Assert.Single(restored.Workspace.Events).Id);
    }

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

        var place = session.CreatePlace("North Gate", "The old stone entrance.", "Gate");
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
        scene = session.AssignSceneToPlace(scene.Id, place.Id);

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

        var originalTimeline = new TimelineDefinition { Name = "Original timeline", Description = "Shared history." };
        var branchTimeline = new TimelineDefinition
        {
            Name = "Changed history",
            Description = "A divergent branch.",
            ParentTimelineId = originalTimeline.Id
        };
        session.Workspace.AddTimeline(originalTimeline);
        session.Workspace.AddTimeline(branchTimeline);

        var planningPoint = session.CreatePlanningPoint(originalTimeline.Id, "Plot Twist", "Near the midpoint", "Reveal a hidden relationship later.");
        var timelineEvent = session.CreateTimelineEvent(originalTimeline.Id, "The reveal", "A hidden relationship is exposed.", "Near the midpoint");
        var manuscript = session.CreateManuscript("The Lost Cartographer", "A book in progress", "T. Best");
        var chapter = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The North Gate");
        session.UpdateManuscriptSection(manuscript.Id, chapter.Sections[0].Id, "The North Gate", "Mara placed her hand on the latch.");
        session.LinkSceneToManuscriptSection(manuscript.Id, chapter.Sections[0].Id, scene.Id);
        session.LinkEventToManuscriptSection(manuscript.Id, chapter.Sections[0].Id, timelineEvent.Id);
        session.LinkEventToSceneBeat(scene.Id, scene.Beats[0].Id, timelineEvent.Id);
        var observerKnowledge = session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = character.Id,
            EventId = timelineEvent.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = timelineEvent.Id,
            Status = CanonStatus.Accepted
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
        Assert.Equal(place.Id, Assert.Single(restoredSession.Workspace.Places).Id);
        Assert.Equal(place.Id, Assert.Single(restoredSession.Workspace.Scenes).PlaceId);
        var restoredManuscript = Assert.Single(restoredSession.Workspace.Manuscripts);
        Assert.Equal(manuscript.Id, restoredManuscript.Id);
        Assert.Equal("The Lost Cartographer", restoredManuscript.Title);
        Assert.Equal("T. Best", restoredManuscript.Author);
        Assert.Equal("Mara placed her hand on the latch.", Assert.Single(restoredManuscript.Sections).Body);
        Assert.Contains(scene.Id, Assert.Single(restoredManuscript.Sections).SceneIds);
        Assert.Contains(timelineEvent.Id, Assert.Single(restoredManuscript.Sections).EventIds);
        var restoredTimelineEvent = Assert.Single(restoredSession.Workspace.Events);
        Assert.Equal(timelineEvent.Id, restoredTimelineEvent.Id);
        Assert.Equal(originalTimeline.Id, restoredTimelineEvent.TimelineId);
        Assert.Equal(TimelinePlacementKind.Approximate, restoredTimelineEvent.PlacementKind);
        var restoredKnowledge = Assert.Single(restoredSession.Workspace.ObserverKnowledge);
        Assert.Equal(observerKnowledge.Id, restoredKnowledge.Id);
        Assert.Equal(character.Id, restoredKnowledge.ObserverId);
        Assert.Equal(timelineEvent.Id, restoredKnowledge.EventId);
        Assert.Equal(ObserverKnowledgeKind.Witnessed, restoredKnowledge.Kind);
        var restoredPoint = Assert.Single(restoredSession.Workspace.PlanningPoints);
        Assert.Equal(planningPoint.Id, restoredPoint.Id);
        Assert.Equal(originalTimeline.Id, restoredPoint.TimelineId);
        Assert.Equal("Plot Twist", restoredPoint.Label);
        Assert.Equal("Near the midpoint", restoredPoint.ApproximatePlacement);
        Assert.Equal(TimelinePlanningStatus.Placeholder, restoredPoint.Status);
        Assert.Equal(TimelinePlacementKind.Approximate, restoredPoint.PlacementKind);
        Assert.Equal(2, restoredSession.Workspace.Timelines.Count);
        Assert.Equal(originalTimeline.Id, restoredSession.Workspace.Timelines[0].Id);
        Assert.Equal(branchTimeline.Id, restoredSession.Workspace.Timelines[1].Id);
        Assert.Equal(originalTimeline.Id, restoredSession.Workspace.Timelines[1].ParentTimelineId);
        Assert.True(restoredSession.Workspace.Timelines[1].IsBranch);
        var restoredPremise = Assert.Single(restoredSession.Workspace.Premises);
        Assert.Equal(premise.Id, restoredPremise.Id);
        Assert.Equal("The Lost Cartographer", restoredPremise.Title);
        Assert.Equal("Connected series", restoredPremise.Scale);
        var restoredCharacter = Assert.Single(restoredSession.Workspace.Characters);
        Assert.Equal(character.Id, restoredCharacter.Id);
        Assert.Equal(CharacterNarrativeRole.Hero, restoredCharacter.NarrativeRole);
        Assert.Equal(scene.Id, Assert.Single(restoredSession.Workspace.Scenes).Id);
        Assert.Equal("left hand on the latch", Assert.Single(restoredSession.Workspace.Scenes[0].Beats).PoseOrPosition);
        Assert.Contains(timelineEvent.Id, Assert.Single(restoredSession.Workspace.Scenes[0].Beats).EventIds);
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
        var legacyCharacter = Assert.Single(document.Workspace.Characters);
        Assert.Equal("Legacy Hero", legacyCharacter.Name);
        Assert.Equal(CharacterNarrativeRole.Undecided, legacyCharacter.NarrativeRole);
    }

    [Fact]
    public void BinaryDocumentRejectsBrokenCrossRecordReferencesBeforeWriting()
    {
        var workspace = new StoryWorkspace { Name = "Broken workspace" };
        workspace.AddEvent(new StoryEvent
        {
            Title = "An event with a missing participant",
            ParticipantIds = [Guid.NewGuid()]
        });
        var document = new StoryWorkspaceBinaryDocument(workspace);
        using var output = new MemoryBinaryStream();

        var exception = Assert.Throws<InvalidDataException>(() => document.Pack(output));

        Assert.Contains("broken reference", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, output.Length);
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
        writer.Write(0); // Experience capabilities
        writer.Write(0); // Experience constraints
        writer.Write(0); // Experience design decisions
        writer.Write(1); // Characters
        writer.Write(Guid.Parse("11111111-1111-1111-1111-111111111111").ToByteArray());
        WriteLegacyString(writer, "Legacy Hero");
        WriteLegacyString(writer, "Scout");
        WriteLegacyString(writer, "A careful explorer.");
        WriteLegacyString(writer, null); // Pronouns
        WriteLegacyString(writer, null); // Species
        writer.Write((int)CanonStatus.Proposed);
        writer.Write(0); // Traits
        writer.Write(0); // Relationships
        writer.Write(0); // Notes
        writer.Write(DateTimeOffset.UnixEpoch.Ticks);
        writer.Write(TimeSpan.Zero.Ticks);
        writer.Write(DateTimeOffset.UnixEpoch.Ticks);
        writer.Write(TimeSpan.Zero.Ticks);
        for (var i = 0; i < 8; i++)
            writer.Write(0); // Scenes, claims, events, campaigns, actions, sources, issues, synonyms
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

    [Fact]
    public void BinaryRestoreRejectsPlaceContainmentCyclesInsteadOfFailingOnInsertionOrder()
    {
        var workspace = new StoryWorkspace();
        var parent = new PlaceRecord { Name = "Parent", Kind = "Region" };
        var child = new PlaceRecord { Name = "Child", Kind = "City", ParentPlaceId = parent.Id };
        workspace.AddPlace(parent);
        workspace.AddPlace(child);
        workspace.ReplacePlace(parent with { ParentPlaceId = child.Id });

        var document = new StoryWorkspaceBinaryDocument(workspace);
        using var output = new MemoryBinaryStream();
        document.Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();

        var exception = Assert.Throws<InvalidDataException>(() => restored.Unpack(input));

        Assert.Contains("containment cycle", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
