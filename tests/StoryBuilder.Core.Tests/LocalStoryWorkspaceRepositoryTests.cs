using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class LocalStoryWorkspaceRepositoryTests
{
    [Fact]
    public async Task Save_and_load_round_trips_workspace_through_fsm_serialization()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);
        var source = new StoryBuilderSession("city: Tacoma");

        try
        {
            await repository.SaveAsync(0x5441434F4D41UL, source);

            var restored = await repository.LoadAsync(0x5441434F4D41UL);

            Assert.NotNull(restored);
            Assert.Equal(source.Workspace.Id, restored!.Workspace.Id);
            Assert.Equal("city: Tacoma", restored.Workspace.Name);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Save_and_load_round_trips_verified_source_identity_map()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);
        var source = new StoryBuilderSession("Lenny");
        var manuscript = source.CreateManuscript("Lenny");
        manuscript = source.AddManuscriptSection(
            manuscript.Id,
            StoryBuilder.Core.Models.ManuscriptSectionKind.Scene,
            "Page 01");
        var sectionId = manuscript.Sections[^1].Id;
        var import = new CompositionManifestImportResult(
            source,
            "experiences/Lenny/SourceManuscript.md",
            "685d275782b76a4c5bdd30d6ae69baeaea919993",
            new Dictionary<string, Guid>(StringComparer.Ordinal) { ["lenny-page-01"] = sectionId });

        try
        {
            await repository.SaveAsync(0x4C454E4E59UL, import);

            var restored = await repository.LoadWithIdentityMapAsync(0x4C454E4E59UL);

            Assert.NotNull(restored);
            Assert.Equal(source.Workspace.Id, restored!.Session.Workspace.Id);
            Assert.Equal(import.SourcePath, restored.SourcePath);
            Assert.Equal(import.SourceBlobSha, restored.SourceBlobSha);
            Assert.Equal(sectionId, restored.SourceSectionIds["lenny-page-01"]);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Map_aware_load_fails_closed_when_workspace_changes_after_map_was_saved()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);
        var source = new StoryBuilderSession("Lenny");
        var manuscript = source.CreateManuscript("Lenny");
        manuscript = source.AddManuscriptSection(
            manuscript.Id,
            StoryBuilder.Core.Models.ManuscriptSectionKind.Scene,
            "Page 01");
        var import = new CompositionManifestImportResult(
            source,
            "experiences/Lenny/SourceManuscript.md",
            "685d275782b76a4c5bdd30d6ae69baeaea919993",
            new Dictionary<string, Guid>(StringComparer.Ordinal) { ["lenny-page-01"] = manuscript.Sections[^1].Id });

        try
        {
            await repository.SaveAsync(0x4C454E4E59UL, import);
            await repository.SaveAsync(0x4C454E4E59UL, new StoryBuilderSession("Changed workspace"));

            await Assert.ThrowsAsync<InvalidDataException>(
                () => repository.LoadWithIdentityMapAsync(0x4C454E4E59UL));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Load_returns_null_when_no_local_working_copy_exists()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);

        try
        {
            Assert.Null(await repository.LoadAsync(0x5441434F4D41UL));
            Assert.Null(await repository.LoadWithIdentityMapAsync(0x5441434F4D41UL));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
