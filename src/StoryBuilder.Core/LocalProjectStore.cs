using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.StoryBuilder;

/// <summary>
/// Explicit local-first project persistence. Writes replace the destination only
/// after a complete temporary file has been flushed.
/// </summary>
public sealed class LocalProjectStore
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SaveAsync(
        string path,
        StoryProject project,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(project);

        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new ArgumentException("A project file directory is required.", nameof(path));
        Directory.CreateDirectory(directory);

        project.UpdatedUtc = DateTimeOffset.UtcNow;
        var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                options: FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, project, Options, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    public async Task<StoryProject> LoadAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path);

        await using var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);

        var project = await JsonSerializer.DeserializeAsync<StoryProject>(
            stream, Options, cancellationToken);

        if (project is null)
            throw new InvalidDataException("The project file did not contain a StoryBuilder project.");
        if (project.SchemaVersion != StoryProject.CurrentSchemaVersion)
            throw new InvalidDataException(
                $"Unsupported StoryBuilder schema {project.SchemaVersion}; expected {StoryProject.CurrentSchemaVersion}.");

        return project;
    }
}
