using System.Text;
using Microsoft.JSInterop;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.WebDemo;

/// <summary>
/// Browser-local persistence coordinator. It stores a versioned binary document as Base64 in localStorage;
/// no JSON or server-side shared workspace is involved.
/// </summary>
public sealed class StoryBuilderPersistenceCoordinator : IDisposable
{
    private readonly StoryBuilderSession _session;
    private readonly IJSRuntime _javascript;
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    private Task? _loadTask;
    private bool _disposed;

    public StoryBuilderPersistenceCoordinator(StoryBuilderSession session, IJSRuntime javascript)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
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
            var document = StoryWorkspaceBinaryDocument.FromSession(_session);
            using var stream = new MemoryBinaryStream();
            document.Pack(stream);
            var encoded = Convert.ToBase64String(stream.ToArray());
            await _javascript.InvokeVoidAsync("storyBuilderStorage.set", encoded);
            LastSavedAtUtc = DateTimeOffset.UtcNow;
            SaveError = null;
        }
        catch (Exception ex) when (IsExpectedPersistenceFailure(ex))
        {
            SaveError = $"Could not save this workspace in browser storage: {ex.Message}";
        }
        finally
        {
            _saveGate.Release();
            NotifyStateChanged();
        }
    }

    /// <summary>Explicitly discards an unreadable browser-local snapshot and starts a fresh workspace.</summary>
    public async Task ClearAndStartFreshAsync()
    {
        if (_disposed)
            return;

        await _javascript.InvokeVoidAsync("storyBuilderStorage.clear");
        _session.RestoreFrom(new StoryWorkspace { Name = "StoryBuilder Web Demo" }, Array.Empty<VocabularyConcept>());
        LoadError = null;
        SaveError = null;
        LastSavedAtUtc = null;
        IsLoaded = true;
        NotifyStateChanged();
        await SaveAsync();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _session.Changed -= OnSessionChanged;
    }

    private async Task LoadCoreAsync()
    {
        try
        {
            var encoded = await _javascript.InvokeAsync<string?>("storyBuilderStorage.get");
            if (!string.IsNullOrWhiteSpace(encoded))
            {
                var bytes = Convert.FromBase64String(encoded);
                using var stream = new MemoryBinaryStream(bytes);
                var document = new StoryWorkspaceBinaryDocument();
                document.Unpack(stream);
                document.RestoreInto(_session);
            }
        }
        catch (Exception ex) when (IsExpectedPersistenceFailure(ex))
        {
            // Preserve the unreadable saved value until the user explicitly chooses to reset it.
            LoadError = $"The saved workspace could not be loaded and has been left untouched: {ex.Message}";
        }
        finally
        {
            IsLoaded = true;
            NotifyStateChanged();
        }
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
            or InvalidOperationException;
}
