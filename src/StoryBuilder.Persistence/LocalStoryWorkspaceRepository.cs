using System.Security.Cryptography;
using System.Text.Json;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.Persistence;

/// <summary>
/// Durable local working-copy storage for editable StoryBuilder MicroBundle content.
/// Published immutable artifacts are deliberately handled by a separate publishing adapter.
/// </summary>
public sealed class LocalStoryWorkspaceRepository
{
    private const int IdentityMapFormatVersion = 1;
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

        var payload = Serialize(session);
        await WriteAtomicallyAsync(GetPath(bundleId), payload, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Saves a workspace and its source identity sidecar. The sidecar is bound to the exact
    /// serialized workspace bytes; a partial or mismatched pair is rejected by map-aware loads.
    /// </summary>
    public async Task SaveAsync(
        ulong bundleId,
        CompositionManifestImportResult import,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(import);
        ArgumentNullException.ThrowIfNull(import.Session);
        ValidateIdentityMap(import.Session, import.SourcePath, import.SourceBlobSha, import.SourceSectionIds);
        cancellationToken.ThrowIfCancellationRequested();

        var payload = Serialize(import.Session);
        var sidecar = new SourceIdentityMapDocument
        {
            FormatVersion = IdentityMapFormatVersion,
            WorkspaceSha256 = Convert.ToHexString(SHA256.HashData(payload)),
            SourcePath = import.SourcePath,
            SourceBlobSha = import.SourceBlobSha,
            SourceSectionIds = import.SourceSectionIds.ToDictionary(
                pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
        };

        Directory.CreateDirectory(_rootDirectory);
        await WriteAtomicallyAsync(GetPath(bundleId), payload, cancellationToken).ConfigureAwait(false);
        var sidecarPayload = JsonSerializer.SerializeToUtf8Bytes(sidecar, JsonOptions);
        await WriteAtomicallyAsync(GetIdentityMapPath(bundleId), sidecarPayload, cancellationToken).ConfigureAwait(false);
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
        return Deserialize(payload);
    }

    /// <summary>
    /// Loads a workspace together with its source identity map. Missing, stale, malformed, or
    /// structurally invalid sidecars fail closed rather than silently losing source identity.
    /// </summary>
    public async Task<StoryWorkspaceWithIdentityMap?> LoadWithIdentityMapAsync(
        ulong bundleId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var workspacePath = GetPath(bundleId);
        if (!File.Exists(workspacePath))
            return null;

        var sidecarPath = GetIdentityMapPath(bundleId);
        if (!File.Exists(sidecarPath))
            throw new InvalidDataException("The workspace has no source identity map sidecar.");

        var payload = await File.ReadAllBytesAsync(workspacePath, cancellationToken).ConfigureAwait(false);
        var sidecarPayload = await File.ReadAllBytesAsync(sidecarPath, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        SourceIdentityMapDocument sidecar;
        try
        {
            sidecar = JsonSerializer.Deserialize<SourceIdentityMapDocument>(sidecarPayload, JsonOptions)
                ?? throw new InvalidDataException("The source identity map sidecar is empty.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The source identity map sidecar is malformed.", exception);
        }

        if (sidecar.FormatVersion != IdentityMapFormatVersion)
            throw new InvalidDataException($"Unsupported source identity map format version: {sidecar.FormatVersion}.");

        var actualHash = Convert.ToHexString(SHA256.HashData(payload));
        if (!string.Equals(actualHash, sidecar.WorkspaceSha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("The source identity map does not match the saved workspace content.");

        var session = Deserialize(payload);
        if (sidecar.SourceSectionIds is null)
            throw new InvalidDataException("The source identity map has no section mappings.");

        ValidateIdentityMap(session, sidecar.SourcePath, sidecar.SourceBlobSha, sidecar.SourceSectionIds);
        return new StoryWorkspaceWithIdentityMap(
            session,
            sidecar.SourcePath,
            sidecar.SourceBlobSha,
            sidecar.SourceSectionIds);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        WriteIndented = true
    };

    private static byte[] Serialize(StoryBuilderSession session)
    {
        var document = StoryWorkspaceBinaryDocument.FromSession(session);
        using var memory = new MemoryBinaryStream();
        document.Pack(memory);
        return memory.ToArray();
    }

    private static StoryBuilderSession Deserialize(byte[] payload)
    {
        using var memory = new MemoryBinaryStream(payload);
        var document = new StoryWorkspaceBinaryDocument();
        try
        {
            document.Unpack(memory);
        }
        catch (InvalidDataException exception) when (
            exception.Message == "The payload is not a StoryBuilder workspace document.")
        {
            throw new InvalidDataException(
                "This local workspace uses the retired SBWS development signature. It is an unpublished development snapshot and cannot be restored. Start a fresh workspace, or remove the local .sbws file and matching .sbmap sidecar only after confirming they are no longer needed.",
                exception);
        }

        var session = new StoryBuilderSession();
        document.RestoreInto(session);
        return session;
    }

    private static void ValidateIdentityMap(
        StoryBuilderSession session,
        string sourcePath,
        string sourceBlobSha,
        IReadOnlyDictionary<string, Guid> sourceSectionIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceBlobSha);
        ArgumentNullException.ThrowIfNull(sourceSectionIds);
        if (sourceSectionIds.Count == 0)
            throw new InvalidDataException("The source identity map cannot be empty.");
        if (sourceSectionIds.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value == Guid.Empty))
            throw new InvalidDataException("The source identity map contains an empty source ID or section ID.");
        if (sourceSectionIds.Values.Distinct().Count() != sourceSectionIds.Count)
            throw new InvalidDataException("The source identity map maps multiple source IDs to the same manuscript section.");

        var sectionIds = session.Workspace.Manuscripts
            .SelectMany(manuscript => manuscript.Sections)
            .Select(section => section.Id)
            .ToHashSet();
        if (sourceSectionIds.Values.Any(sectionId => !sectionIds.Contains(sectionId)))
            throw new InvalidDataException("The source identity map references a manuscript section missing from the workspace.");
    }

    private async Task WriteAtomicallyAsync(
        string destination,
        byte[] payload,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootDirectory);
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

    private string GetPath(ulong bundleId) =>
        Path.Combine(_rootDirectory, $"bundle-{bundleId:X16}.sbws");

    private string GetIdentityMapPath(ulong bundleId) =>
        Path.Combine(_rootDirectory, $"bundle-{bundleId:X16}.sbmap");

    private sealed class SourceIdentityMapDocument
    {
        public int FormatVersion { get; set; }
        public string WorkspaceSha256 { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string SourceBlobSha { get; set; } = string.Empty;
        public Dictionary<string, Guid> SourceSectionIds { get; set; } = new(StringComparer.Ordinal);
    }
}

/// <summary>Restored workspace plus the verified source-manifest identity mapping.</summary>
public sealed record StoryWorkspaceWithIdentityMap(
    StoryBuilderSession Session,
    string SourcePath,
    string SourceBlobSha,
    IReadOnlyDictionary<string, Guid> SourceSectionIds);
