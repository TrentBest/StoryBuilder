using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.FSM;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class NarratorAgentTests
{
    [Fact]
    public void NarratorCoordinatesIndependentIntentPlanningObserverAndDraftMachines()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var character = session.CreateCharacter("Mara");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The gate opens", "Mara witnesses the gate open.", "Early in the story");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = character.Id,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = storyEvent.Id,
            Status = CanonStatus.Accepted
        });
        var manuscript = session.CreateManuscript("The Lost Cartographer");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The North Gate");

        using var narrator = new NarratorAgent(session);
        var premise = narrator.CaptureIntent("A cartographer discovers a hidden history.");
        var point = narrator.AddPlanningPoint(timeline.Id, "Plot Twist", "Near the midpoint");
        narrator.MarkPlanSpecified();
        narrator.SelectObserver(character.Id);
        var evaluation = narrator.EvaluateObserverClaim(storyEvent.Id, storyEvent.Id, NarrationClaimMode.DirectFact);
        narrator.RequestDraft(manuscript.Id, section.Sections[0].Id, character.Id);
        var submission = narrator.SubmitDraft("Mara watched the gate open.", [storyEvent.Id], storyEvent.Id);

        Assert.Equal(premise.Id, narrator.CurrentPremiseId);
        Assert.Equal("Captured", narrator.IntentState);
        Assert.Equal(point.Id, narrator.LastPlanningPointId);
        Assert.Equal("Specified", narrator.PlanningState);
        Assert.Equal("Allowed", narrator.ObserverState);
        Assert.True(evaluation.IsAllowed);
        Assert.Equal("Ready", narrator.DraftState);
        Assert.True(submission.Accepted);
        Assert.Contains(storyEvent.Id, session.Workspace.Manuscripts.Single().Sections.Single().EventIds);
        Assert.True(narrator.Events.Count >= 8);
    }

    [Fact]
    public void NarratorHoldsDraftWhenObserverCannotStateReportedEventAsDirectFact()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var character = session.CreateCharacter("Mara");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The gate opens", "The gate opens out of sight.", "Near the midpoint");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = character.Id,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Told,
            AcquiredAtEventId = storyEvent.Id,
            Attribution = "Mara's sister",
            Status = CanonStatus.Accepted
        });
        var manuscript = session.CreateManuscript("The Lost Cartographer");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The Rumor");

        using var narrator = new NarratorAgent(session);
        narrator.RequestDraft(manuscript.Id, section.Sections[0].Id, character.Id);
        var submission = narrator.SubmitDraft("Mara saw the gate open.", [storyEvent.Id], storyEvent.Id, NarrationClaimMode.DirectFact);

        Assert.False(submission.Accepted);
        Assert.Equal("NeedsReview", narrator.DraftState);
        Assert.Empty(session.Workspace.Manuscripts.Single().Sections.Single().Body);
        Assert.Equal(NarrationClaimMode.AttributedReport, submission.Constraints[0].RequiredMode);
    }

    [Fact]
    public void SeparateNarratorAgentsDoNotShareIntentState()
    {
        var firstSession = new StoryBuilderSession();
        var secondSession = new StoryBuilderSession();
        using var first = new NarratorAgent(firstSession);
        using var second = new NarratorAgent(secondSession);

        first.CaptureIntent("First idea.");

        Assert.Equal("Captured", first.IntentState);
        Assert.Equal("Idle", second.IntentState);
        Assert.NotEmpty(first.Events);
        Assert.Empty(secondSession.Workspace.Premises);
    }
}
