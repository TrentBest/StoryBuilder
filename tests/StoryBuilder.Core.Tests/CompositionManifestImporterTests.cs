using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class CompositionManifestImporterTests
{
    [Fact]
    public void ImportsAllLennyPagesInOrderAndCreatesLinkedVisualTargets()
    {
        var pages = string.Join(",", Enumerable.Range(1, 33).Select(number =>
            $$"""{"pageNumber":{{number}},"id":"lenny-page-{{number:00}}","text":"Exact page {{number}} text."}"""));
        var json = $$"""
        {
          "schemaVersion": 1,
          "title": "Little Lenny and the Big Ocean Problem",
          "source": {
            "path": "experiences/Lenny/SourceManuscript.md",
            "blobSha": "0123456789012345678901234567890123456789"
          },
          "dedication": { "id": "lenny-dedication", "text": "For the ocean." },
          "storyPages": [{{pages}}],
          "closingPage": { "id": "lenny-closing", "text": "The end." }
        }
        """;

        var session = CompositionManifestImporter.Import(json);
        var manuscript = Assert.Single(session.Workspace.Manuscripts);
        var ordered = manuscript.Sections.OrderBy(section => section.Sequence).ToArray();

        Assert.Equal(35, ordered.Length);
        Assert.Equal(ManuscriptSectionKind.FrontMatter, ordered[0].Kind);
        Assert.Equal("For the ocean.", ordered[0].Body);
        for (var index = 0; index < 33; index++)
        {
            Assert.Equal($"Page {index + 1:00}", ordered[index + 1].Title);
            Assert.Equal($"Exact page {index + 1} text.", ordered[index + 1].Body);
        }
        Assert.Equal(ManuscriptSectionKind.BackMatter, ordered[^1].Kind);
        Assert.Equal("The end.", ordered[^1].Body);

        Assert.Equal(5, session.Workspace.Characters.Count);
        Assert.Equal(5, session.Workspace.Places.Count);
        Assert.Equal(43, session.Workspace.VisualAssets.Count);
        Assert.All(session.Workspace.VisualAssets.Where(asset => asset.Kind == VisualAssetKind.PageIllustration),
            asset => Assert.NotNull(asset.ManuscriptSectionId));
        Assert.All(session.Workspace.Characters, character =>
            Assert.Contains("provisional", character.Notes["Visual design"], StringComparison.OrdinalIgnoreCase));
        Assert.Single(session.Workspace.Sources);
    }

    [Fact]
    public void RejectsManifestWithMissingOrOutOfOrderPages()
    {
        const string json = """
        {
          "title": "Lenny",
          "source": { "path": "source.md", "blobSha": "abc" },
          "dedication": { "text": "Dedication" },
          "storyPages": [
            { "pageNumber": 2, "id": "page-2", "text": "Page two" }
          ],
          "closingPage": { "text": "Closing" }
        }
        """;

        Assert.Throws<InvalidDataException>(() => CompositionManifestImporter.Import(json));
    }
}
