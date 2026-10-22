using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.WebDemo;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderTimelineRendererTests
{
    [Fact]
    public void TimelineMapShowsEventToBookSectionPortrayalRail()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal", "The hidden truth emerges.", "Midpoint");
        var manuscript = session.CreateManuscript("The Lost Cartographer", author: "T. Best");
        var chapter = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The North Gate");
        session.LinkEventToManuscriptSection(manuscript.Id, chapter.Sections[0].Id, storyEvent.Id);

        var svg = new StoryBuilderTimelineRenderer().Render(session);

        Assert.Contains("BOOK PORTRAYAL RAIL", svg, StringComparison.Ordinal);
        Assert.Contains("E1: The", svg, StringComparison.Ordinal);
        Assert.Contains("The Lost Cartographer: The North Gate", svg, StringComparison.Ordinal);
    }
    [Fact]
    public void TimelineMapShowsEventToSceneBeatPortrayalRail()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal", "The hidden truth emerges.", "Midpoint");
        var scene = session.CreateScene("The Vault");
        scene = session.UpdateScene(scene.AddBeat(new SceneBeat
        {
            Kind = SceneBeatKind.Action,
            Action = "breaks the seal",
            Status = CanonStatus.Accepted
        }));
        session.LinkEventToSceneBeat(scene.Id, scene.Beats[0].Id, storyEvent.Id);

        var svg = new StoryBuilderTimelineRenderer().Render(session);

        Assert.Contains("SCENE BEAT RAIL", svg, StringComparison.Ordinal);
        Assert.Contains("The Vault b1", svg, StringComparison.Ordinal);
        Assert.Contains("breaks the seal", svg, StringComparison.Ordinal);
    }
}
