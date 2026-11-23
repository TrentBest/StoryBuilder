using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class NarrationDraftRequestBuilderTests
{
    [Fact]
    public void BuildSnapshotsOnlySelectedBeatsInSceneOrderAndManuscriptContext()
    {
        var session = new StoryBuilderSession();
        var scene = session.CreateScene("The gate", "North wall");
        var earlierBeat = new SceneBeat
        {
            Sequence = 1,
            Kind = SceneBeatKind.Action,
            ActorLabel = "Mara",
            Action = "approaches the gate"
        };
        var laterBeat = new SceneBeat
        {
            Sequence = 2,
            Kind = SceneBeatKind.Dialogue,
            ActorLabel = "Mara",
            Dialogue = "Open it."
        };
        session.UpdateScene(scene with { Beats = [laterBeat, earlierBeat] });
        var manuscript = session.CreateManuscript("The Chronicle");
        manuscript = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The North Wall");
        var section = manuscript.Sections.Single() with { Body = "Existing passage." };
        session.UpdateManuscript(manuscript.ReplaceSection(section));

        var request = new NarrationDraftRequestBuilder().Build(
            session,
            scene.Id,
            [laterBeat.Id, earlierBeat.Id],
            manuscript.Id,
            section.Id,
            voice: "  restrained  ",
            style: "  historical  ");

        Assert.Equal(scene.Id, request.SceneId);
        Assert.Equal("The gate", request.SceneTitle);
        Assert.Equal("North wall", request.Location);
        Assert.Equal(manuscript.Id, request.ManuscriptId);
        Assert.Equal("The Chronicle", request.ManuscriptTitle);
        Assert.Equal(section.Id, request.SectionId);
        Assert.Equal("Existing passage.", request.ExistingSectionText);
        Assert.Equal("restrained", request.Voice);
        Assert.Equal("historical", request.Style);
        Assert.Equal(new[] { earlierBeat.Id, laterBeat.Id }, request.SourceBeats.Select(beat => beat.BeatId));
    }

    [Fact]
    public void BuildRejectsSelectedBeatThatBelongsToAnotherScene()
    {
        var session = new StoryBuilderSession();
        var scene = session.CreateScene("The gate");
        var otherScene = session.CreateScene("The tower");
        var foreignBeat = new SceneBeat { Sequence = 1, Action = "rings the bell" };
        session.UpdateScene(otherScene with { Beats = [foreignBeat] });
        var manuscript = session.CreateManuscript();
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter);

        Assert.Throws<KeyNotFoundException>(() => new NarrationDraftRequestBuilder().Build(
            session,
            scene.Id,
            [foreignBeat.Id],
            manuscript.Id,
            section.Sections.Single().Id));
    }

    [Fact]
    public void ObserverConstrainedRequestIncludesOnlyBeatsSupportedByObserverKnowledge()
    {
        var session = new StoryBuilderSession();
        var observer = session.CreateCharacter("Mara");
        var timeline = session.CreateTimeline("Main");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The gate opens");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = storyEvent.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            Attribution = "Mara",
            Status = CanonStatus.Accepted
        });
        var scene = session.CreateScene("At the gate");
        var beat = new SceneBeat
        {
            Sequence = 1,
            Kind = SceneBeatKind.Action,
            ActorId = observer.Id,
            ActorLabel = "Mara",
            Action = "opens the gate",
            EventIds = [storyEvent.Id]
        };
        session.UpdateScene(scene with { Beats = [beat] });
        var manuscript = session.CreateManuscript("Chronicle");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter);

        var request = new NarrationDraftRequestBuilder().Build(
            session,
            scene.Id,
            [beat.Id],
            manuscript.Id,
            section.Sections.Single().Id,
            observerId: observer.Id);

        Assert.Equal(observer.Id, request.ObserverId);
        Assert.NotNull(request.ObserverKnowledge);
        Assert.Contains(request.ObserverKnowledge.Events, item => item.EventId == storyEvent.Id);
        Assert.Equal(new[] { storyEvent.Id }, request.SourceBeats.Single().EventIds);
    }

    [Fact]
    public void ObserverConstrainedRequestRejectsUnlinkedBeatRatherThanLeakingItsContents()
    {
        var session = new StoryBuilderSession();
        var observer = session.CreateCharacter("Mara");
        var scene = session.CreateScene("At the gate");
        var beat = new SceneBeat
        {
            Sequence = 1,
            Action = "secretly unlocks the hidden door"
        };
        session.UpdateScene(scene with { Beats = [beat] });
        var manuscript = session.CreateManuscript("Chronicle");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter);

        Assert.Throws<InvalidOperationException>(() => new NarrationDraftRequestBuilder().Build(
            session,
            scene.Id,
            [beat.Id],
            manuscript.Id,
            section.Sections.Single().Id,
            observerId: observer.Id));
    }

    [Fact]
    public void BuildRejectsDuplicateBeatSelection()
    {
        var session = new StoryBuilderSession();
        var scene = session.CreateScene("The gate");
        var beat = new SceneBeat { Sequence = 1, Action = "opens" };
        session.UpdateScene(scene with { Beats = [beat] });
        var manuscript = session.CreateManuscript();
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter);

        Assert.Throws<ArgumentException>(() => new NarrationDraftRequestBuilder().Build(
            session,
            scene.Id,
            [beat.Id, beat.Id],
            manuscript.Id,
            section.Sections.Single().Id));
    }
}
