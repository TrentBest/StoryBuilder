using System.Text;
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
    public void WritesUtf8MarkdownToCallerOwnedStream()
    {
        var session = new StoryBuilderSession();
        var manuscript = session.CreateManuscript("Exported book");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Opening");
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Opening", "The door opens.");
        using var destination = new MemoryStream();

        var artifact = new BookProductRenderer().WriteMarkdown(session.Workspace, manuscript.Id, destination);

        Assert.Equal(artifact.Markdown, Encoding.UTF8.GetString(destination.ToArray()));
        Assert.True(destination.CanWrite);
        Assert.StartsWith("# Exported book", artifact.Markdown, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsReadOnlyMarkdownDestination()
    {
        using var destination = new MemoryStream(Array.Empty<byte>(), writable: false);
        var session = new StoryBuilderSession();
        var manuscript = session.CreateManuscript("Read only");

        Assert.Throws<ArgumentException>(() => new BookProductRenderer().WriteMarkdown(session.Workspace, manuscript.Id, destination));
    }

    [Fact]
    public void RejectsUnknownManuscript()
    {
        var renderer = new BookProductRenderer();
        Assert.Throws<KeyNotFoundException>(() => renderer.Render(new StoryWorkspace(), Guid.NewGuid()));
    }
}
