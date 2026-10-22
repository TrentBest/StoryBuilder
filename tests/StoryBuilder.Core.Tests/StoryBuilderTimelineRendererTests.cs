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
        Assert.Contains("The reveal", svg, StringComparison.Ordinal);
        Assert.Contains("The Lost Cartographer: The North Gate", svg, StringComparison.Ordinal);
    }
}
