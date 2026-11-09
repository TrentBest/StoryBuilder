using Microsoft.JSInterop;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;

namespace StoryBuilder.WebDemo;

/// <summary>
/// Host-persistent workspace coordinator. Existing browser-local snapshots are imported once
/// when no host workspace exists, then removed only after the host save succeeds.
/// </summary>
public sealed class StoryBuilderPersistenceCoordinator : IDisposable
{
    private const ulong WorkspaceId = 1;
    private readonly StoryBuilderSession _session;
    private readonly LocalStoryWorkspaceRepository _repository;
    private readonly IJSRuntime _javascript;
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    private Task? _loadTask;
    private bool _disposed;
    private bool _removeLegacySnapshotAfterSave;

    public StoryBuilderPersistenceCoordinator(
        StoryBuilderSession session,
        LocalStoryWorkspaceRepository repository,
        IJSRuntime javascript)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _javascript = javascript ?? throw new ArgumentNullException(nameof(javascript));
        _session.Changed += OnSessionChanged;
    }

    public bool IsLoaded { get; private set; }
    public string? LoadError { get; private set; }
    public string? SaveError { get; private set; }
    public DateTimeOffset? LastSavedAtUtc { get; private set; }
    public event Action? StateChanged;

    public Task EnsureLoadedAsync() => _loadTask ??= LoadCoreAsync();

    public async Task SaveAsync()
    {
        if (_disposed || !IsLoaded || LoadError is not null)
            return;

        await _saveGate.WaitAsync();
        try
        {
            await _repository.SaveAsync(WorkspaceId, _session);
            LastSavedAtUtc = DateTimeOffset.UtcNow;
            SaveError = null;

            if (_removeLegacySnapshotAfterSave)
            {
                await _javascript.InvokeVoidAsync("storyBuilderStorage.clear");
                _removeLegacySnapshotAfterSave = false;
            }
        }
        catch (Exception ex) when (IsExpectedPersistenceFailure(ex))
        {
            SaveError = $"Could not save this workspace to host storage: {ex.Message}";
        }
        finally
        {
            _saveGate.Release();
            NotifyStateChanged();
        }
    }

    /// <summary>Explicitly replaces the current workspace with a fresh empty workspace.</summary>
    public async Task ClearAndStartFreshAsync()
    {
        if (_disposed)
            return;

        _session.RestoreFrom(new StoryWorkspace { Name = "StoryBuilder Web Demo" }, Array.Empty<VocabularyConcept>());
        LoadError = null;
        SaveError = null;
        LastSavedAtUtc = null;
        IsLoaded = true;
        _removeLegacySnapshotAfterSave = true;
        NotifyStateChanged();
        await SaveAsync();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _session.Changed -= OnSessionChanged;
        _saveGate.Dispose();
    }

    private async Task LoadCoreAsync()
    {
        try
        {
            var savedSession = await _repository.LoadAsync(WorkspaceId);
            if (savedSession is not null)
            {
                _session.RestoreFrom(savedSession.Workspace, savedSession.Vocabulary.ExportConcepts());
            }
            else
            {
                // Backward-compatible import for workspaces saved by older browser-only builds.
                var encoded = await _javascript.InvokeAsync<string?>("storyBuilderStorage.get");
                if (!string.IsNullOrWhiteSpace(encoded))
                {
                    var bytes = Convert.FromBase64String(encoded);
                    var document = new StoryWorkspaceBinaryDocument();
                    document.Unpack(new TheSingularityWorkshop.FSM_Serialization.MemoryBinaryStream(bytes));
                    document.RestoreInto(_session);
                    _removeLegacySnapshotAfterSave = true;
                }
            }
        }
        catch (Exception ex) when (IsExpectedPersistenceFailure(ex))
        {
            // Do not overwrite an unreadable host or browser snapshot automatically.
            LoadError = $"The saved workspace could not be loaded and has been left untouched: {ex.Message}";
        }
        finally
        {
            IsLoaded = true;
            NotifyStateChanged();
        }

        if (_removeLegacySnapshotAfterSave && LoadError is null)
            await SaveAsync();
    }
    private void OnSessionChanged()
    {
        if (IsLoaded && LoadError is null && !_disposed)
            _ = SaveAsync();
    }

    private void NotifyStateChanged() => StateChanged?.Invoke();

    private static bool IsExpectedPersistenceFailure(Exception exception) =>
        exception is JSException
            or IOException
            or InvalidDataException
            or ArgumentException
            or FormatException
            or InvalidOperationException
            or UnauthorizedAccessException;
}
