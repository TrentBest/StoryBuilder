using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class SceneBeatEventLinkTests
{
    [Fact]
    public void LinkingIsIdempotentAndUnlinkingPreservesEventAndBeat()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal");
        var scene = session.CreateScene("The Vault");
        scene = session.UpdateScene(scene.AddBeat(new SceneBeat { Action = "breaks the seal" }));
        var beatId = scene.Beats[0].Id;

        session.LinkEventToSceneBeat(scene.Id, beatId, storyEvent.Id);
        var linked = session.LinkEventToSceneBeat(scene.Id, beatId, storyEvent.Id);
        Assert.Equal(new[] { storyEvent.Id }, Assert.Single(linked.Beats).EventIds);

        var unlinked = session.LinkEventToSceneBeat(scene.Id, beatId, storyEvent.Id, linked: false);

        Assert.Empty(Assert.Single(unlinked.Beats).EventIds);
        Assert.Contains(session.Workspace.Events, item => item.Id == storyEvent.Id);
        Assert.Contains(session.Workspace.Scenes, item => item.Id == scene.Id);
    }

    [Fact]
    public void LinkingRejectsUnknownSceneBeatOrEvent()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal");
        var scene = session.CreateScene("The Vault");

        Assert.Throws<KeyNotFoundException>(() => session.LinkEventToSceneBeat(scene.Id, Guid.NewGuid(), storyEvent.Id));
        Assert.Throws<KeyNotFoundException>(() => session.LinkEventToSceneBeat(scene.Id, Guid.NewGuid(), Guid.NewGuid()));
        Assert.Throws<KeyNotFoundException>(() => session.LinkEventToSceneBeat(Guid.NewGuid(), Guid.NewGuid(), storyEvent.Id));
    }
}
