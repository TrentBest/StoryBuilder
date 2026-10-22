using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class CompositionPackageBinaryArtifactsTests
{
    [Fact]
    public void CompiledPackageValidatesAndRestoresTheExactWorkspace()
    {
        var session = new StoryBuilderSession("Book package");
        session.CreateTimeline("Main timeline");
        var manuscript = session.CreateManuscript("The Example Book", author: "T. Best");
        var chapter = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "First chapter");
        session.UpdateManuscriptSection(manuscript.Id, chapter.Sections[0].Id, "First chapter", "A real first page.");

        var artifact = new CompositionPackageCompiler().Compile(
            session,
            "revision-1",
            "book",
            Guid.NewGuid(),
            [new MicroBundleRequirement { BundleId = 42, Version = "1.0.0", RequiredCapabilities = ["book.render"] }],
            [new MicroBundleRuntimeConfiguration
            {
                BundleId = 42,
                BundleVersion = "1.0.0",
                Settings = [new MicroBundleSetting { Key = "page-size", ValueType = "string", Value = "trade" }]
            }]);

        var result = new CompositionPackageReader().ValidateAndRestore(artifact, [42UL], ["book.render"]);

        Assert.True(result.IsValid, string.Join(Environment.NewLine, result.Errors));
        Assert.NotNull(result.Session);
        Assert.Equal(session.Workspace.Id, result.Session!.Workspace.Id);
        Assert.Equal("The Example Book", Assert.Single(result.Session.Workspace.Manuscripts).Title);
        Assert.Equal("A real first page.", Assert.Single(Assert.Single(result.Session.Workspace.Manuscripts).Sections).Body);
        Assert.Equal("trade", Assert.Single(Assert.Single(result.Configuration!.Bundles).Settings).Value);
    }

    [Fact]
    public void TamperedContentIsRejectedBeforeWorkspaceRestore()
    {
        var session = new StoryBuilderSession("Tamper test");
        var artifact = new CompositionPackageCompiler().Compile(
            session,
            "revision-1",
            "book",
            Guid.NewGuid(),
            [],
            []);
        var tampered = artifact with { ContentPayload = [.. artifact.ContentPayload, 0x7f] };

        var result = new CompositionPackageReader().ValidateAndRestore(tampered, [], []);

        Assert.False(result.IsValid);
        Assert.Null(result.Session);
        Assert.Contains(result.Errors, error => error.Contains("content payload", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingRequiredBundleIsRejectedBeforeRestore()
    {
        var artifact = new CompositionPackageCompiler().Compile(
            new StoryBuilderSession(),
            "revision-1",
            "book",
            Guid.NewGuid(),
            [new MicroBundleRequirement { BundleId = 42, Version = "1.0.0" }],
            [new MicroBundleRuntimeConfiguration { BundleId = 42, BundleVersion = "1.0.0" }]);

        var result = new CompositionPackageReader().ValidateAndRestore(artifact, [], []);

        Assert.False(result.IsValid);
        Assert.Null(result.Session);
        Assert.Contains(result.Errors, error => error.Contains("not available", StringComparison.Ordinal));
    }

    [Fact]
    public void RepeatedPackageConsumptionProducesTheSameBookWithoutReArbitration()
    {
        var session = new StoryBuilderSession("Package product");
        var timeline = session.CreateTimeline("Main history");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The ink awakens");
        var manuscript = session.CreateManuscript("The Lost Cartographer", author: "T. Best");
        var chapter = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Chapter One");
        session.UpdateManuscriptSection(manuscript.Id, chapter.Sections[0].Id, "Chapter One", "The ink remembers.");
        session.LinkEventToManuscriptSection(manuscript.Id, chapter.Sections[0].Id, storyEvent.Id);
        var arbitrationRecordId = Guid.NewGuid();
        var artifact = new CompositionPackageCompiler().Compile(
            session, "revision-1", "book", arbitrationRecordId, [], []);

        var reader = new CompositionPackageReader();
        var firstLoad = reader.ValidateAndRestore(artifact, [], []);
        var secondLoad = reader.ValidateAndRestore(artifact, [], []);

        Assert.True(firstLoad.IsValid, string.Join(Environment.NewLine, firstLoad.Errors));
        Assert.True(secondLoad.IsValid, string.Join(Environment.NewLine, secondLoad.Errors));
        var renderer = new BookProductRenderer();
        var firstBook = renderer.Render(firstLoad.Session!.Workspace, manuscript.Id);
        var secondBook = renderer.Render(secondLoad.Session!.Workspace, manuscript.Id);
        Assert.Equal(firstBook.Markdown, secondBook.Markdown);
        Assert.Equal(firstBook.ReferencedEventIds, secondBook.ReferencedEventIds);
        Assert.Equal(arbitrationRecordId, firstLoad.Manifest!.ArbitrationRecordId);
        Assert.Equal(arbitrationRecordId, secondLoad.Manifest!.ArbitrationRecordId);
    }

}
