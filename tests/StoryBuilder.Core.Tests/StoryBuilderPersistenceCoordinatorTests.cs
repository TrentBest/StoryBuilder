using Microsoft.JSInterop;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using StoryBuilder.WebDemo;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderPersistenceCoordinatorTests
{
    [Fact]
    public async Task CoordinatorPersistsWorkspaceOnHostAndRestoresItIntoANewSession()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        var javascript = new FakeJsRuntime();
        var repository = new LocalStoryWorkspaceRepository(root);
        try
        {
            var firstSession = new StoryBuilderSession("Host save");
            using (var firstCoordinator = new StoryBuilderPersistenceCoordinator(firstSession, repository, javascript))
            {
                await firstCoordinator.EnsureLoadedAsync();
                firstSession.CreateCharacter("Mara");
                await firstCoordinator.SaveAsync();

                Assert.Null(firstCoordinator.SaveError);
                Assert.NotNull(firstCoordinator.LastSavedAtUtc);
                Assert.True(Directory.GetFiles(root, "*.sbws").Length == 1);
            }

            var restoredSession = new StoryBuilderSession("Empty session");
            using var restoredCoordinator = new StoryBuilderPersistenceCoordinator(restoredSession, repository, javascript);
            await restoredCoordinator.EnsureLoadedAsync();

            Assert.Equal("Host save", restoredSession.Workspace.Name);
            Assert.Equal("Mara", Assert.Single(restoredSession.Workspace.Characters).Name);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CoordinatorMigratesLegacyBrowserSnapshotToHostBeforeClearingIt()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        var javascript = new FakeJsRuntime();
        var legacySession = new StoryBuilderSession("Legacy browser save");
        legacySession.CreateCharacter("Lenny");
        var legacyDocument = StoryWorkspaceBinaryDocument.FromSession(legacySession);
        using var memory = new TheSingularityWorkshop.FSM_Serialization.MemoryBinaryStream();
        legacyDocument.Pack(memory);
        javascript.SavedValue = Convert.ToBase64String(memory.ToArray());

        try
        {
            var restoredSession = new StoryBuilderSession("Empty");
            using var coordinator = new StoryBuilderPersistenceCoordinator(
                restoredSession, new LocalStoryWorkspaceRepository(root), javascript);
            await coordinator.EnsureLoadedAsync();

            Assert.Equal("Legacy browser save", restoredSession.Workspace.Name);
            Assert.Equal("Lenny", Assert.Single(restoredSession.Workspace.Characters).Name);
            Assert.Null(javascript.SavedValue);
            Assert.Null(coordinator.SaveError);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CoordinatorPreservesLegacyBrowserSnapshotWhenHostMigrationSaveFails()
    {
        var root = Path.Combine(Path.GetTempPath(), $"StoryBuilderTests-{Guid.NewGuid():N}");
        await File.WriteAllTextAsync(root, "not a directory");

        var javascript = new FakeJsRuntime();
        var legacySession = new StoryBuilderSession("Legacy browser save");
        legacySession.CreateCharacter("Lenny");
        var legacyDocument = StoryWorkspaceBinaryDocument.FromSession(legacySession);
        using var memory = new TheSingularityWorkshop.FSM_Serialization.MemoryBinaryStream();
        legacyDocument.Pack(memory);
        javascript.SavedValue = Convert.ToBase64String(memory.ToArray());

        try
        {
            var restoredSession = new StoryBuilderSession("Empty");
            using var coordinator = new StoryBuilderPersistenceCoordinator(
                restoredSession, new LocalStoryWorkspaceRepository(root), javascript);
            await coordinator.EnsureLoadedAsync();

            Assert.Equal("Legacy browser save", restoredSession.Workspace.Name);
            Assert.Equal("Lenny", Assert.Single(restoredSession.Workspace.Characters).Name);
            Assert.NotNull(javascript.SavedValue);
            Assert.NotNull(coordinator.SaveError);
            Assert.True(File.Exists(root));
        }
        finally
        {
            if (File.Exists(root))
                File.Delete(root);
        }
    }


    [Fact]
    public async Task CoordinatorPreservesMalformedLegacySnapshotUntilExplicitFreshStart()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        var javascript = new FakeJsRuntime { SavedValue = "not-valid-base64%%%" };
        try
        {
            var session = new StoryBuilderSession("Initial");
            using var coordinator = new StoryBuilderPersistenceCoordinator(session, new LocalStoryWorkspaceRepository(root), javascript);
            await coordinator.EnsureLoadedAsync();

            Assert.NotNull(coordinator.LoadError);
            Assert.Equal("not-valid-base64%%%", javascript.SavedValue);
            Assert.Empty(Directory.Exists(root) ? Directory.GetFiles(root) : Array.Empty<string>());

            await coordinator.ClearAndStartFreshAsync();
            Assert.Null(coordinator.LoadError);
            Assert.Null(coordinator.SaveError);
            Assert.Null(javascript.SavedValue);
            Assert.True(File.Exists(Path.Combine(root, "bundle-0000000000000001.sbws")));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CoordinatorPreservesLegacySnapshotWhenBrowserStorageIsDenied()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        var javascript = new FakeJsRuntime { ThrowOnGet = true };
        try
        {
            var session = new StoryBuilderSession("Initial");
            using var coordinator = new StoryBuilderPersistenceCoordinator(session, new LocalStoryWorkspaceRepository(root), javascript);
            await coordinator.EnsureLoadedAsync();

            Assert.NotNull(coordinator.LoadError);
            Assert.True(javascript.ThrowOnGet);
            Assert.False(Directory.Exists(root));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CoordinatorPreservesUnreadableHostWorkspaceUntilExplicitFreshStart()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var workspaceFile = Path.Combine(root, "bundle-0000000000000001.sbws");
        var corruptBytes = new byte[] { 0x53, 0x42, 0x57, 0x53, 0xFF, 0x00, 0x13 };
        await File.WriteAllBytesAsync(workspaceFile, corruptBytes);

        try
        {
            var session = new StoryBuilderSession("Initial");
            using var coordinator = new StoryBuilderPersistenceCoordinator(session, new LocalStoryWorkspaceRepository(root), new FakeJsRuntime());
            await coordinator.EnsureLoadedAsync();

            Assert.NotNull(coordinator.LoadError);
            Assert.Equal(corruptBytes, await File.ReadAllBytesAsync(workspaceFile));

            await coordinator.ClearAndStartFreshAsync();
            Assert.Null(coordinator.LoadError);
            Assert.Null(coordinator.SaveError);

            var restored = await new LocalStoryWorkspaceRepository(root).LoadAsync(1);
            Assert.NotNull(restored);
            Assert.Equal("StoryBuilder Web Demo", restored!.Workspace.Name);
            Assert.Empty(restored.Workspace.Characters);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    private sealed class FakeJsRuntime : IJSRuntime
    {
        public string? SavedValue { get; set; }
        public bool ThrowOnGet { get; set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            object? result;
            switch (identifier)
            {
                case "storyBuilderStorage.get":
                    if (ThrowOnGet)
                        throw new JSException("Browser storage access was denied.");
                    result = SavedValue;
                    break;
                case "storyBuilderStorage.clear":
                    SavedValue = null;
                    result = default(TValue);
                    break;
                default:
                    throw new JSException($"Unexpected JavaScript identifier: {identifier}");
            }

            return ValueTask.FromResult((TValue)result!);
        }
    }
}
