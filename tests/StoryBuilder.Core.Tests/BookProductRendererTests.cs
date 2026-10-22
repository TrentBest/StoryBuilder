using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class BookProductRendererTests
{
    [Fact]
    public void RendersOrderedManuscriptAsDeterministicDigitalBook()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The ink awakens");
        var manuscript = session.CreateManuscript(
            "The Lost Cartographer",
            subtitle: "An atlas of impossible places",
            author: "T. Best");
        var chapter = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Chapter One");
        session.UpdateManuscriptSection(manuscript.Id, chapter.Sections[0].Id, "Chapter One", "The ink remembers.\r\nA second line.");
        session.LinkEventToManuscriptSection(manuscript.Id, chapter.Sections[0].Id, storyEvent.Id);

        var renderer = new BookProductRenderer();
        var first = renderer.Render(session.Workspace, manuscript.Id);
        var second = renderer.Render(session.Workspace, manuscript.Id);

        Assert.StartsWith("# The Lost Cartographer\n\nAn atlas of impossible places", first.Markdown, StringComparison.Ordinal);
        Assert.Contains("## Chapter One\n\nThe ink remembers.\nA second line.", first.Markdown, StringComparison.Ordinal);
        Assert.Equal(first.Markdown, second.Markdown);
        Assert.Equal(new[] { storyEvent.Id }, first.ReferencedEventIds);
        Assert.Equal(first.ReferencedEventIds, second.ReferencedEventIds);
        Assert.Equal(1, first.SectionCount);
    }

    [Fact]
    public void RejectsUnknownManuscript()
    {
        var renderer = new BookProductRenderer();
        Assert.Throws<KeyNotFoundException>(() => renderer.Render(new StoryWorkspace(), Guid.NewGuid()));
    }
}
