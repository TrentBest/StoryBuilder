using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class VesperReachAuthoredWorkspaceTests
{
    [Fact]
    public void FirstContactWorkspacePreservesDistinctObserverKnowledgeAndAuthoredLinksAcrossPersistence()
    {
        var session = new StoryBuilderSession("Vesper Reach — The Signal Nobody Owns");
        var timeline = session.CreateTimeline("Vesper Reach station chronology");

        var ilyan = session.CreateCharacter("Ilyan Voss", "Station signal analyst")
            .WithDescription("Examined the incoming signal evidence directly.");
        var mara = session.CreateCharacter("Mara Quill", "Courier")
            .WithDescription("Heard the station's official bulletin.");
        var teren = session.CreateCharacter("Teren", "Berth mechanic")
            .WithDescription("Was isolated in a berth and received neither account.");
        session.UpdateCharacter(ilyan);
        session.UpdateCharacter(mara);
        session.UpdateCharacter(teren);

        var signalSource = session.AddSource(new SourceRecord
        {
            Title = "Frontier signal instrument record",
            SourceType = "Instrument record",
            Perspective = "Station signal analysis",
            ReliabilityNote = "Primary measurements; interpretation remains uncertain."
        });
        var bulletinSource = session.AddSource(new SourceRecord
        {
            Title = "Vesper Reach official lounge bulletin",
            SourceType = "Public bulletin",
            Perspective = "Station administration",
            ReliabilityNote = "An attributed institutional account, not independent observation."
        });
        var independentSource = session.AddSource(new SourceRecord
        {
            Title = "Courier's independent relay account",
            SourceType = "Witness account",
            Perspective = "Independent courier report"
        });

        var signal = session.CreateTimelineEvent(
            timeline.Id,
            "An anomalous signal is detected beyond the settled frontier.",
            "Station instruments record an anomalous signal. Its origin and meaning remain unresolved.",
            "Station shift 41");
        signal = signal with
        {
            ParticipantIds = [ilyan.Id],
            SourceIds = [signalSource.Id],
            Status = CanonStatus.Accepted
        };
        session.Workspace.ReplaceEvent(signal);

        var bulletin = session.AddEvent(new StoryEvent
        {
            Title = "The station publishes its initial signal bulletin.",
            Summary = "The lounge display reports the anomalous signal and attributes the measurements to station analysis.",
            TimelineId = timeline.Id,
            InWorldDateOrPeriod = "Station shift 41, shortly after detection",
            CauseEventIds = [signal.Id],
            SourceIds = [bulletinSource.Id],
            Status = CanonStatus.Accepted
        });

        var independentAccount = session.AddEvent(new StoryEvent
        {
            Title = "An independent courier account is recorded.",
            Summary = "A later account offers a separate perspective; it does not rewrite the original bulletin.",
            TimelineId = timeline.Id,
            InWorldDateOrPeriod = "Station shift 42",
            CauseEventIds = [signal.Id],
            SourceIds = [independentSource.Id],
            Status = CanonStatus.Accepted
        });

        session.AddHistoricalClaim(new HistoricalClaim
        {
            Statement = "Station instruments recorded an anomalous signal beyond the settled frontier.",
            InWorldDateOrPeriod = "Station shift 41",
            Attribution = "Station instrument record",
            Confidence = 0.95,
            Status = CanonStatus.Accepted,
            SourceIds = [signalSource.Id],
            RelatedEntityIds = [signal.Id]
        });
        session.AddHistoricalClaim(new HistoricalClaim
        {
            Statement = "The official bulletin reported the anomalous signal to the station.",
            InWorldDateOrPeriod = "Station shift 41",
            Attribution = "Vesper Reach station administration",
            Confidence = 0.8,
            Status = CanonStatus.Accepted,
            SourceIds = [bulletinSource.Id],
            RelatedEntityIds = [bulletin.Id, signal.Id]
        });
        session.AddHistoricalClaim(new HistoricalClaim
        {
            Statement = "A courier later recorded an independent account of the signal report.",
            InWorldDateOrPeriod = "Station shift 42",
            Attribution = "Courier account",
            Confidence = 0.6,
            Status = CanonStatus.Proposed,
            SourceIds = [independentSource.Id],
            RelatedEntityIds = [independentAccount.Id, signal.Id]
        });

        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = ilyan.Id,
            EventId = signal.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = signal.Id,
            SourceId = signalSource.Id,
            Note = "I examined the station instrument record directly.",
            Status = CanonStatus.Accepted
        });
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = mara.Id,
            EventId = signal.Id,
            Kind = ObserverKnowledgeKind.Told,
            AcquiredAtEventId = bulletin.Id,
            SourceId = bulletinSource.Id,
            Attribution = "Vesper Reach official lounge bulletin",
            Note = "The station bulletin reported an anomalous signal beyond the frontier.",
            Status = CanonStatus.Accepted
        });

        var place = session.CreatePlace("Vesper Reach", "A frontier station beyond settled space.", "Station");
        var scene = session.CreateScene("The lounge bulletin", "Station lounge", SceneCaptureMode.Claymation)
            .AddBeat(new SceneBeat
            {
                Kind = SceneBeatKind.Action,
                ActorId = mara.Id,
                ActorLabel = mara.Name,
                Action = "hears the official bulletin while crossing the lounge",
                Status = CanonStatus.Accepted
            });
        session.UpdateScene(scene);
        session.AssignSceneToPlace(scene.Id, place.Id);

        var manuscript = session.CreateManuscript("The Signal Nobody Owns", "First-contact story at Vesper Reach.");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Scene, "The lounge");
        session.LinkEventToManuscriptSection(manuscript.Id, section.Sections[0].Id, signal.Id);
        session.LinkEventToManuscriptSection(manuscript.Id, section.Sections[0].Id, bulletin.Id);
        session.LinkEventToManuscriptSection(manuscript.Id, section.Sections[0].Id, independentAccount.Id);
        session.LinkSceneToManuscriptSection(manuscript.Id, section.Sections[0].Id, scene.Id);

        var projections = new ObserverKnowledgeProjectionService();
        var ilyanProjection = projections.Project(session.Workspace, ilyan.Id, bulletin.Id);
        var maraProjection = projections.Project(session.Workspace, mara.Id, bulletin.Id);
        var terenProjection = projections.Project(session.Workspace, teren.Id, bulletin.Id);

        Assert.Contains(ilyanProjection.Events, item => item.EventId == signal.Id &&
            item.PermittedClaims.Any(claim => claim.Mode == NarrationClaimMode.DirectFact));
        var maraSignalKnowledge = Assert.Single(maraProjection.Events, item => item.EventId == signal.Id);
        Assert.Contains(maraSignalKnowledge.PermittedClaims, claim => claim.Mode == NarrationClaimMode.AttributedReport);
        Assert.DoesNotContain(maraSignalKnowledge.PermittedClaims, claim => claim.Mode == NarrationClaimMode.DirectFact);
        Assert.Empty(terenProjection.Events);
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));

        var document = StoryWorkspaceBinaryDocument.FromSession(session);
        using var output = new MemoryBinaryStream();
        document.Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        Assert.Equal(3, restored.Workspace.Characters.Count);
        Assert.Equal(3, restored.Workspace.Sources.Count);
        Assert.Equal(3, restored.Workspace.HistoricalClaims.Count);
        Assert.Equal(2, restored.Workspace.ObserverKnowledge.Count);

        var restoredSignal = Assert.Single(restored.Workspace.Events,
            item => item.Title.StartsWith("An anomalous"));
        Assert.Equal(signal.Id, restoredSignal.Id);
        Assert.Equal(ilyan.Id, Assert.Single(restoredSignal.ParticipantIds));
        Assert.Equal(signalSource.Id, Assert.Single(restoredSignal.SourceIds));

        var restoredBulletin = Assert.Single(restored.Workspace.Events, item => item.Id == bulletin.Id);
        Assert.Equal(signal.Id, Assert.Single(restoredBulletin.CauseEventIds));
        Assert.Equal(bulletinSource.Id, Assert.Single(restoredBulletin.SourceIds));

        var restoredIndependentClaim = Assert.Single(restored.Workspace.HistoricalClaims,
            item => item.Attribution == "Courier account");
        Assert.Equal(CanonStatus.Proposed, restoredIndependentClaim.Status);
        Assert.Equal(independentSource.Id, Assert.Single(restoredIndependentClaim.SourceIds));

        var restoredIlyanKnowledge = Assert.Single(restored.Workspace.ObserverKnowledge,
            item => item.ObserverId == ilyan.Id);
        Assert.Equal(ObserverKnowledgeKind.Witnessed, restoredIlyanKnowledge.Kind);
        Assert.Equal(signal.Id, restoredIlyanKnowledge.AcquiredAtEventId);

        var restoredMaraKnowledge = Assert.Single(restored.Workspace.ObserverKnowledge,
            item => item.ObserverId == mara.Id);
        Assert.Equal(ObserverKnowledgeKind.Told, restoredMaraKnowledge.Kind);
        Assert.Equal(bulletin.Id, restoredMaraKnowledge.AcquiredAtEventId);
        Assert.Equal(bulletinSource.Id, restoredMaraKnowledge.SourceId);

        var restoredPlace = Assert.Single(restored.Workspace.Places, item => item.Id == place.Id);
        var restoredScene = Assert.Single(restored.Workspace.Scenes, item => item.Id == scene.Id);
        Assert.Equal(restoredPlace.Id, restoredScene.PlaceId);
        Assert.Equal(mara.Id, Assert.Single(restoredScene.Beats).ActorId);

        var restoredSection = Assert.Single(restored.Workspace.Manuscripts.SelectMany(item => item.Sections));
        Assert.Contains(signal.Id, restoredSection.EventIds);
        Assert.Contains(bulletin.Id, restoredSection.EventIds);
        Assert.Contains(independentAccount.Id, restoredSection.EventIds);
        Assert.Contains(scene.Id, restoredSection.SceneIds);
        Assert.Empty(StoryWorkspaceValidator.Validate(restored.Workspace));
    }
}
