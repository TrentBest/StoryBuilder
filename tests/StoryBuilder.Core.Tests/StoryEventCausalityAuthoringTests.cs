using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryEventCausalityAuthoringTests
{
    [Fact]
    public void CausalLinksAreReciprocalAndCanBeRemovedWithoutDeletingEvents()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var cause = session.CreateTimelineEvent(timeline.Id, "The signal is sent");
        var consequence = session.CreateTimelineEvent(timeline.Id, "The fleet arrives");

        session.LinkEventCausality(cause.Id, consequence.Id);
        Assert.Contains(consequence.Id, Assert.Single(session.Workspace.Events, item => item.Id == cause.Id).ConsequenceEventIds);
        Assert.Contains(cause.Id, Assert.Single(session.Workspace.Events, item => item.Id == consequence.Id).CauseEventIds);
        Assert.Empty(StoryContinuityAnalyzer.Analyze(session.Workspace));

        session.LinkEventCausality(cause.Id, consequence.Id, linked: false);
        Assert.Empty(Assert.Single(session.Workspace.Events, item => item.Id == cause.Id).ConsequenceEventIds);
        Assert.Empty(Assert.Single(session.Workspace.Events, item => item.Id == consequence.Id).CauseEventIds);
        Assert.Equal(2, session.Workspace.Events.Count);
    }

    [Fact]
    public void EventUpdatesRefreshVocabularyAndRejectSelfCausation()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The beacon", "A quiet signal.");

        var updated = session.UpdateTimelineEvent(storyEvent with { Title = "The warning beacon", Summary = "An urgent signal." });

        Assert.Equal("The warning beacon", updated.Title);
        Assert.Empty(session.Vocabulary.Search("The beacon"));
        Assert.Contains(session.Vocabulary.Search("warning"), usage => usage.OccurrenceCount > 0);
        Assert.Throws<InvalidOperationException>(() => session.LinkEventCausality(updated.Id, updated.Id));
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }

    [Fact]
    public void CausalLinkingRejectsMissingEventsWithoutChangingExistingRecords()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var cause = session.CreateTimelineEvent(timeline.Id, "The signal");

        Assert.Throws<KeyNotFoundException>(() => session.LinkEventCausality(cause.Id, Guid.NewGuid()));
        Assert.Empty(Assert.Single(session.Workspace.Events).ConsequenceEventIds);
    }
}
