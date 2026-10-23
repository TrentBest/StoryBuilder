using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;

namespace StoryBuilder.FSM_COS;

/// <summary>
/// Owns one local editable StoryBuilder experience. It restores from the local repository before
/// composition and saves the working copy during explicit save or orderly asynchronous shutdown.
/// </summary>
public sealed class StoryBuilderLocalExperienceSession : IAsyncDisposable
{
    private readonly LocalStoryWorkspaceRepository _repository;
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);
    private bool _disposed;

    private StoryBuilderLocalExperienceSession(
        LocalStoryWorkspaceRepository repository,
        StoryBuilderHostedExperience hosted)
    {
        _repository = repository;
        Hosted = hosted;
    }

    public StoryBuilderHostedExperience Hosted { get; }

    public static async Task<StoryBuilderLocalExperienceSession> OpenAsync(
        StoryBuilderExperienceHost host,
        LocalStoryWorkspaceRepository repository,
        string kind,
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(repository);
        cancellationToken.ThrowIfCancellationRequested();

        var identity = new StoryBuilderExperienceMicroBundle(kind, name);
        var session = await repository.LoadAsync(identity.Id, cancellationToken).ConfigureAwait(false)
            ?? identity.Session;
        cancellationToken.ThrowIfCancellationRequested();

        var hosted = host.Create(kind, name, session);
        return new StoryBuilderLocalExperienceSession(repository, hosted);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _lifecycleGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            await _repository.SaveAsync(
                Hosted.Experience.Id,
                Hosted.Experience.Session,
                cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    /// <summary>
    /// Saves the editable working copy before releasing this session. If saving fails, disposal
    /// reports the failure rather than silently losing the user's latest changes.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _lifecycleGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_disposed)
                return;

            await _repository.SaveAsync(
                Hosted.Experience.Id,
                Hosted.Experience.Session).ConfigureAwait(false);
            _disposed = true;
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }
}
