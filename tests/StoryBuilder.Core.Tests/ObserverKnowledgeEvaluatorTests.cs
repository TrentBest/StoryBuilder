using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class ObserverKnowledgeEvaluatorTests
{
    [Fact]
    public void WitnessedEventCanBeStatedAtAcquisitionAndCausallyLater()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "Main history" };
        workspace.AddTimeline(timeline);
        var witnessed = new StoryEvent { Title = "The gate opens", TimelineId = timeline.Id };
        workspace.AddEvent(witnessed);
        var later = new StoryEvent
        {
            Title = "The alarm sounds",
            TimelineId = timeline.Id,
            CauseEventIds = [witnessed.Id]
        };
        workspace.AddEvent(later);
        var observerId = Guid.NewGuid();
        workspace.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observerId,
            EventId = witnessed.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = witnessed.Id,
            Status = CanonStatus.Accepted
        });

        var evaluator = new ObserverKnowledgeEvaluator();

        Assert.True(evaluator.Evaluate(workspace, observerId, witnessed.Id, witnessed.Id, NarrationClaimMode.DirectFact).IsAllowed);
        Assert.True(evaluator.Evaluate(workspace, observerId, witnessed.Id, later.Id, NarrationClaimMode.DirectFact).IsAllowed);
    }

    [Fact]
    public void ReportedEventRequiresAttributionInsteadOfDirectFact()
    {
        var workspace = CreateWorkspaceWithEvent(out var storyEvent);
        var observerId = Guid.NewGuid();
        workspace.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observerId,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Told,
            AcquiredAtEventId = storyEvent.Id,
            Attribution = "Mara's sister",
            Status = CanonStatus.Accepted
        });

        var evaluator = new ObserverKnowledgeEvaluator();

        Assert.False(evaluator.Evaluate(workspace, observerId, storyEvent.Id, storyEvent.Id, NarrationClaimMode.DirectFact).IsAllowed);
        Assert.True(evaluator.Evaluate(workspace, observerId, storyEvent.Id, storyEvent.Id, NarrationClaimMode.AttributedReport).IsAllowed);
    }

    [Fact]
    public void ProposedKnowledgeRemainsUnresolvedUntilAccepted()
    {
        var workspace = CreateWorkspaceWithEvent(out var storyEvent);
        var observerId = Guid.NewGuid();
        workspace.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observerId,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = storyEvent.Id,
            Status = CanonStatus.Proposed
        });

        var result = new ObserverKnowledgeEvaluator().Evaluate(
            workspace, observerId, storyEvent.Id, storyEvent.Id, NarrationClaimMode.DirectFact);

        Assert.False(result.IsAllowed);
        Assert.True(result.IsUnresolved);
    }

    [Fact]
    public void UnknownCausalAvailabilityFailsClosed()
    {
        var workspace = CreateWorkspaceWithEvent(out var knownEvent);
        var unrelated = new StoryEvent { Title = "Unrelated point", TimelineId = knownEvent.TimelineId };
        workspace.AddEvent(unrelated);
        var observerId = Guid.NewGuid();
        workspace.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observerId,
            EventId = knownEvent.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = knownEvent.Id,
            Status = CanonStatus.Accepted
        });

        var result = new ObserverKnowledgeEvaluator().Evaluate(
            workspace, observerId, knownEvent.Id, unrelated.Id, NarrationClaimMode.DirectFact);

        Assert.False(result.IsAllowed);
        Assert.Contains("not established as known", result.Explanation);
    }

    private static StoryWorkspace CreateWorkspaceWithEvent(out StoryEvent storyEvent)
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "Main history" };
        workspace.AddTimeline(timeline);
        storyEvent = new StoryEvent { Title = "The gate opens", TimelineId = timeline.Id };
        workspace.AddEvent(storyEvent);
        return workspace;
    }
}
