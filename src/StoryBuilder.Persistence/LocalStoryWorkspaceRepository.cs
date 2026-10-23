using StoryBuilder.Core.Services;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.Persistence;

/// <summary>
/// Durable local working-copy storage for editable StoryBuilder MicroBundle content.
/// Published immutable artifacts are deliberately handled by a separate publishing adapter.
/// </summary>
public sealed class LocalStoryWorkspaceRepository
{
    private readonly string _rootDirectory;

    public LocalStoryWorkspaceRepository(string rootDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }

    /// <summary>Saves a validated workspace using FSM_Serialization's binary stream contract.</summary>
    public async Task SaveAsync(
        ulong bundleId,
        StoryBuilderSession session,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        cancellationToken.ThrowIfCancellationRequested();

        var document = StoryWorkspaceBinaryDocument.FromSession(session);
        using var memory = new MemoryBinaryStream();
        document.Pack(memory);
        var payload = memory.ToArray();

        Directory.CreateDirectory(_rootDirectory);
        var destination = GetPath(bundleId);
        var temporary = destination + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await using (var stream = new FileStream(
                temporary,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                options: FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(payload, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporary, destination, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary))
                File.Delete(temporary);
        }
    }

    /// <summary>Loads and validates a saved workspace, or returns null when no local copy exists.</summary>
    public async Task<StoryBuilderSession?> LoadAsync(
        ulong bundleId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetPath(bundleId);
        if (!File.Exists(path))
            return null;

        var payload = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        using var memory = new MemoryBinaryStream(payload);
        var document = new StoryWorkspaceBinaryDocument();
        document.Unpack(memory);

        var session = new StoryBuilderSession();
        document.RestoreInto(session);
        return session;
    }

    private string GetPath(ulong bundleId) =>
        Path.Combine(_rootDirectory, $"bundle-{bundleId:X16}.sbws");
}
