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
    public async Task Load_returns_null_when_no_local_working_copy_exists()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);

        try
        {
            Assert.Null(await repository.LoadAsync(0x5441434F4D41UL));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
