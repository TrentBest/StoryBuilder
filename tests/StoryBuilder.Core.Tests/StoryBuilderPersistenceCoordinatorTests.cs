using Microsoft.JSInterop;
using StoryBuilder.Core.Services;
using StoryBuilder.WebDemo;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderPersistenceCoordinatorTests
{
    [Fact]
    public async Task CoordinatorSavesBinaryWorkspaceAndRestoresItIntoANewSession()
    {
        var javascript = new FakeJsRuntime();
        var firstSession = new StoryBuilderSession("Browser save");
        using (var firstCoordinator = new StoryBuilderPersistenceCoordinator(firstSession, javascript))
        {
            await firstCoordinator.EnsureLoadedAsync();
            firstSession.CreateCharacter("Mara");
            await firstCoordinator.SaveAsync();

            Assert.NotNull(javascript.SavedValue);
            Assert.DoesNotContain("{", javascript.SavedValue!);
            Assert.NotNull(firstCoordinator.LastSavedAtUtc);
        }

        var restoredSession = new StoryBuilderSession("Empty session");
        using var restoredCoordinator = new StoryBuilderPersistenceCoordinator(restoredSession, javascript);
        await restoredCoordinator.EnsureLoadedAsync();

        Assert.Equal("Browser save", restoredSession.Workspace.Name);
        Assert.Equal("Mara", Assert.Single(restoredSession.Workspace.Characters).Name);
    }

    private sealed class FakeJsRuntime : IJSRuntime
    {
        public string? SavedValue { get; private set; }

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
                    result = SavedValue;
                    break;
                case "storyBuilderStorage.set":
                    SavedValue = args?.FirstOrDefault() as string;
                    result = default(TValue);
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
