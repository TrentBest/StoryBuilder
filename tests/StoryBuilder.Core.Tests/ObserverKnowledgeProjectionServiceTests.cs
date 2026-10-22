using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class ObserverKnowledgeProjectionServiceTests
{
    [Fact]
    public void ProjectionIncludesOnlyAcceptedKnowledgeAndDoesNotCopyWorldSummaries()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var observer = session.CreateCharacter("Mara");
        var known = session.CreateTimelineEvent(timeline.Id, "The gate opens", "Unrestricted world summary must not be copied.");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = known.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = known.Id,
            Note = "I watched the gate open.",
            Status = CanonStatus.Accepted
        });
        var unknown = session.CreateTimelineEvent(timeline.Id, "The hidden vault", "The observer does not know this.");

        var projection = new ObserverKnowledgeProjectionService().Project(session.Workspace, observer.Id, known.Id);

        var projected = Assert.Single(projection.Events);
        Assert.Equal(known.Id, projected.EventId);
        Assert.DoesNotContain(projection.Events, item => item.EventId == unknown.Id);
        Assert.Contains(projected.PermittedClaims, claim =>
            claim.Mode == NarrationClaimMode.DirectFact && claim.Note == "I watched the gate open.");
        Assert.DoesNotContain(typeof(ObserverKnownEvent).GetProperties(), property => property.Name is "Title" or "Summary");
    }

    [Fact]
    public void ProjectionRespectsTheCausalAcquisitionAnchorAndClaimMode()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var observer = session.CreateCharacter("Mara");
        var witnessed = session.CreateTimelineEvent(timeline.Id, "The gate opens", "Mara witnesses the gate open.");
        var reported = session.AddEvent(new StoryEvent
        {
            TimelineId = timeline.Id,
            Title = "The vault is empty",
            Summary = "A later event not directly witnessed by Mara.",
            CauseEventIds = [witnessed.Id]
        });
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = witnessed.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = witnessed.Id,
            Note = "I watched the gate open.",
            Status = CanonStatus.Accepted
        });
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = reported.Id,
            Kind = ObserverKnowledgeKind.Told,
            AcquiredAtEventId = reported.Id,
            Attribution = "Mara's sister",
            Note = "My sister says the vault was empty.",
            Status = CanonStatus.Accepted
        });

        var beforeReport = new ObserverKnowledgeProjectionService().Project(session.Workspace, observer.Id, witnessed.Id);
        var afterReport = new ObserverKnowledgeProjectionService().Project(session.Workspace, observer.Id, reported.Id);

        Assert.Single(beforeReport.Events);
        Assert.Equal(2, afterReport.Events.Count);
        var report = Assert.Single(afterReport.Events.Where(item => item.EventId == reported.Id));
        Assert.Contains(report.PermittedClaims, claim => claim.Mode == NarrationClaimMode.AttributedReport);
        Assert.DoesNotContain(report.PermittedClaims, claim => claim.Mode == NarrationClaimMode.DirectFact);
    }

    [Fact]
    public void ProposedKnowledgeDoesNotEnterTheObserverProjection()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var observer = session.CreateCharacter("Mara");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "A rumored arrival");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Told,
            AcquiredAtEventId = storyEvent.Id,
            Attribution = "Unknown source",
            Status = CanonStatus.Proposed
        });

        var projection = new ObserverKnowledgeProjectionService().Project(session.Workspace, observer.Id, storyEvent.Id);

        Assert.Empty(projection.Events);
    }
}
