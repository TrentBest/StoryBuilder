using System.Security.Cryptography;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.Persistence;

/// <summary>Three exact binary payloads that a host can store as manifest, configuration, and content artifacts.</summary>
public sealed record CompositionPackageArtifact(byte[] ManifestPayload, byte[] ConfigurationPayload, byte[] ContentPayload);

/// <summary>Compiles an already-arbitrated workspace into versioned binary artifacts; it does not run arbitration.</summary>
public sealed class CompositionPackageCompiler
{
    public CompositionPackageArtifact Compile(
        StoryBuilderSession session,
        string contentRevision,
        string outputProfile,
        Guid arbitrationRecordId,
        IEnumerable<MicroBundleRequirement> bundleRequirements,
        IEnumerable<MicroBundleRuntimeConfiguration> bundleConfigurations)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRevision);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputProfile);
        if (arbitrationRecordId == Guid.Empty)
            throw new ArgumentException("A completed Workshop arbitration record is required.", nameof(arbitrationRecordId));
        ArgumentNullException.ThrowIfNull(bundleRequirements);
        ArgumentNullException.ThrowIfNull(bundleConfigurations);

        var requirements = bundleRequirements.ToList();
        var configurations = bundleConfigurations.ToList();
        var packageId = Guid.NewGuid();
        var configuration = new CompositionPackageConfiguration
        {
            PackageId = packageId,
            WorkspaceId = session.Workspace.Id,
            ContentRevision = contentRevision,
            OutputProfile = outputProfile.Trim(),
            Bundles = configurations
        };
        var contentPayload = PackWorkspace(session);
        var configurationPayload = CompositionPackageBinaryCodec.PackConfiguration(configuration);
        var manifest = new CompositionPackageManifest
        {
            PackageId = packageId,
            WorkspaceId = session.Workspace.Id,
            ContentRevision = contentRevision,
            ContentSha256 = Convert.ToHexString(SHA256.HashData(contentPayload)),
            ConfigurationSha256 = Convert.ToHexString(SHA256.HashData(configurationPayload)),
            OutputProfile = outputProfile.Trim(),
            ArbitrationRecordId = arbitrationRecordId,
            Bundles = requirements
        };
        var manifestPayload = CompositionPackageBinaryCodec.PackManifest(manifest);
        return new CompositionPackageArtifact(manifestPayload, configurationPayload, contentPayload);
    }

    private static byte[] PackWorkspace(StoryBuilderSession session)
    {
        using var stream = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(stream);
        return stream.ToArray();
    }
}

/// <summary>Validates the captured package before restoring its content; no host arbitration is performed.</summary>
public sealed class CompositionPackageReader
{
    public CompositionPackageLoadResult ValidateAndRestore(
        CompositionPackageArtifact artifact,
        IEnumerable<ulong> availableBundleIds,
        IEnumerable<string> availableCapabilities)
    {
        ArgumentNullException.ThrowIfNull(artifact);
        ArgumentNullException.ThrowIfNull(availableBundleIds);
        ArgumentNullException.ThrowIfNull(availableCapabilities);

        CompositionPackageManifest manifest;
        CompositionPackageConfiguration configuration;
        try
        {
            manifest = CompositionPackageBinaryCodec.UnpackManifest(artifact.ManifestPayload);
            configuration = CompositionPackageBinaryCodec.UnpackConfiguration(artifact.ConfigurationPayload);
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or ArgumentException)
        {
            return CompositionPackageLoadResult.Failed([$"Manifest or configuration could not be read: {exception.Message}"]);
        }

        var errors = new CompositionPackageValidator().Validate(
            manifest,
            configuration,
            Convert.ToHexString(SHA256.HashData(artifact.ContentPayload)),
            Convert.ToHexString(SHA256.HashData(artifact.ConfigurationPayload)),
            availableBundleIds,
            availableCapabilities);
        if (errors.Count > 0)
            return CompositionPackageLoadResult.Failed(errors, manifest, configuration);

        try
        {
            using var stream = new MemoryBinaryStream(artifact.ContentPayload);
            var document = new StoryWorkspaceBinaryDocument();
            document.Unpack(stream);
            if (document.Workspace.Id != manifest.WorkspaceId)
                return CompositionPackageLoadResult.Failed(["The restored content workspace identity does not match the manifest."], manifest, configuration);
            var session = new StoryBuilderSession();
            document.RestoreInto(session);
            return new CompositionPackageLoadResult(true, [], manifest, configuration, session);
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or ArgumentException or KeyNotFoundException)
        {
            return CompositionPackageLoadResult.Failed([$"Content payload could not be restored: {exception.Message}"], manifest, configuration);
        }
    }
}

/// <summary>Result of validating a package and, only on success, restoring its workspace.</summary>
public sealed record CompositionPackageLoadResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    CompositionPackageManifest? Manifest,
    CompositionPackageConfiguration? Configuration,
    StoryBuilderSession? Session)
{
    public static CompositionPackageLoadResult Failed(
        IReadOnlyList<string> errors,
        CompositionPackageManifest? manifest = null,
        CompositionPackageConfiguration? configuration = null) =>
        new(false, errors, manifest, configuration, null);
}

internal static class CompositionPackageBinaryCodec
{
    private static readonly byte[] ManifestMagic = "SBPM"u8.ToArray();
    private static readonly byte[] ConfigurationMagic = "SBPC"u8.ToArray();
    private const int ManifestFormatVersion = 2;
    private const int ConfigurationFormatVersion = 1;

    internal static byte[] PackManifest(CompositionPackageManifest value) => Pack(stream =>
    {
        stream.Write(ManifestMagic);
        BinaryCodec.WriteInt32(stream, ManifestFormatVersion);
        BinaryCodec.WriteInt32(stream, value.SchemaVersion);
        BinaryCodec.WriteGuid(stream, value.PackageId);
        BinaryCodec.WriteGuid(stream, value.WorkspaceId);
        BinaryCodec.WriteString(stream, value.ContentRevision);
        BinaryCodec.WriteString(stream, value.ContentSha256);
        BinaryCodec.WriteString(stream, value.ConfigurationSha256);
        BinaryCodec.WriteString(stream, value.OutputProfile);
        BinaryCodec.WriteGuid(stream, value.ArbitrationRecordId);
        BinaryCodec.WriteList(stream, value.Bundles, (target, item) =>
        {
            BinaryCodec.WriteInt64(target, unchecked((long)item.BundleId));
            BinaryCodec.WriteString(target, item.Version);
            BinaryCodec.WriteBoolean(target, item.Required);
            BinaryCodec.WriteStringList(target, item.RequiredCapabilities);
            BinaryCodec.WriteCount(target, item.Dependencies.Count);
            foreach (var dependencyId in item.Dependencies)
                BinaryCodec.WriteInt64(target, unchecked((long)dependencyId));
        });
    });

    internal static CompositionPackageManifest UnpackManifest(byte[] payload) => Unpack(payload, stream =>
    {
        ReadMagic(stream, ManifestMagic, "manifest");
        ReadFormatVersion(stream, ManifestFormatVersion);
        var schemaVersion = BinaryCodec.ReadInt32(stream);
        var packageId = BinaryCodec.ReadGuid(stream);
        var workspaceId = BinaryCodec.ReadGuid(stream);
        var contentRevision = BinaryCodec.ReadRequiredString(stream, "ContentRevision");
        var contentHash = BinaryCodec.ReadRequiredString(stream, "ContentSha256");
        var configHash = BinaryCodec.ReadRequiredString(stream, "ConfigurationSha256");
        var outputProfile = BinaryCodec.ReadRequiredString(stream, "OutputProfile");
        var arbitrationId = BinaryCodec.ReadGuid(stream);
        var bundles = BinaryCodec.ReadList(stream, source => new MicroBundleRequirement
        {
            BundleId = unchecked((ulong)BinaryCodec.ReadInt64(source)),
            Version = BinaryCodec.ReadRequiredString(source, "BundleVersion"),
            Required = BinaryCodec.ReadBoolean(source),
            RequiredCapabilities = BinaryCodec.ReadStringList(source),
            Dependencies = ReadDependencies(source)
        });
        return new CompositionPackageManifest
        {
            SchemaVersion = schemaVersion,
            PackageId = packageId,
            WorkspaceId = workspaceId,
            ContentRevision = contentRevision,
            ContentSha256 = contentHash,
            ConfigurationSha256 = configHash,
            OutputProfile = outputProfile,
            ArbitrationRecordId = arbitrationId,
            Bundles = bundles
        };
    });

    internal static byte[] PackConfiguration(CompositionPackageConfiguration value) => Pack(stream =>
    {
        stream.Write(ConfigurationMagic);
        BinaryCodec.WriteInt32(stream, ConfigurationFormatVersion);
        BinaryCodec.WriteInt32(stream, value.SchemaVersion);
        BinaryCodec.WriteGuid(stream, value.PackageId);
        BinaryCodec.WriteGuid(stream, value.WorkspaceId);
        BinaryCodec.WriteString(stream, value.ContentRevision);
        BinaryCodec.WriteString(stream, value.OutputProfile);
        BinaryCodec.WriteList(stream, value.Bundles, (target, item) =>
        {
            BinaryCodec.WriteInt64(target, unchecked((long)item.BundleId));
            BinaryCodec.WriteString(target, item.BundleVersion);
            BinaryCodec.WriteBoolean(target, item.Enabled);
            BinaryCodec.WriteList(target, item.Settings, (settingStream, setting) =>
            {
                BinaryCodec.WriteString(settingStream, setting.Key);
                BinaryCodec.WriteString(settingStream, setting.ValueType);
                BinaryCodec.WriteString(settingStream, setting.Value);
            });
        });
    });

    internal static CompositionPackageConfiguration UnpackConfiguration(byte[] payload) => Unpack(payload, stream =>
    {
        ReadMagic(stream, ConfigurationMagic, "configuration");
        ReadFormatVersion(stream, ConfigurationFormatVersion);
        var schemaVersion = BinaryCodec.ReadInt32(stream);
        var packageId = BinaryCodec.ReadGuid(stream);
        var workspaceId = BinaryCodec.ReadGuid(stream);
        var contentRevision = BinaryCodec.ReadRequiredString(stream, "ContentRevision");
        var outputProfile = BinaryCodec.ReadRequiredString(stream, "OutputProfile");
        var bundles = BinaryCodec.ReadList(stream, source => new MicroBundleRuntimeConfiguration
        {
            BundleId = unchecked((ulong)BinaryCodec.ReadInt64(source)),
            BundleVersion = BinaryCodec.ReadRequiredString(source, "BundleVersion"),
            Enabled = BinaryCodec.ReadBoolean(source),
            Settings = BinaryCodec.ReadList(source, settingStream => new MicroBundleSetting
            {
                Key = BinaryCodec.ReadRequiredString(settingStream, "Setting.Key"),
                ValueType = BinaryCodec.ReadRequiredString(settingStream, "Setting.ValueType"),
                Value = BinaryCodec.ReadRequiredString(settingStream, "Setting.Value")
            })
        });
        return new CompositionPackageConfiguration
        {
            SchemaVersion = schemaVersion,
            PackageId = packageId,
            WorkspaceId = workspaceId,
            ContentRevision = contentRevision,
            OutputProfile = outputProfile,
            Bundles = bundles
        };
    });

    private static byte[] Pack(Action<IBinaryStream> write)
    {
        using var stream = new MemoryBinaryStream();
        write(stream);
        return stream.ToArray();
    }

    private static T Unpack<T>(byte[] payload, Func<IBinaryStream, T> read)
    {
        ArgumentNullException.ThrowIfNull(payload);
        using var stream = new MemoryBinaryStream(payload);
        var result = read(stream);
        if (stream.Position != stream.Length)
            throw new InvalidDataException("The package artifact contains trailing bytes.");
        return result;
    }

    private static void ReadMagic(IBinaryStream stream, byte[] expected, string artifactName)
    {
        Span<byte> actual = stackalloc byte[4];
        BinaryCodec.ReadExactly(stream, actual);
        if (!actual.SequenceEqual(expected))
            throw new InvalidDataException($"The payload is not a StoryBuilder {artifactName} artifact.");
    }

    private static List<ulong> ReadDependencies(IBinaryStream stream)
    {
        var count = BinaryCodec.ReadCount(stream);
        var dependencies = new List<ulong>(count);
        for (var index = 0; index < count; index++)
            dependencies.Add(unchecked((ulong)BinaryCodec.ReadInt64(stream)));
        return dependencies;
    }

    private static void ReadFormatVersion(IBinaryStream stream, int expectedVersion)
    {
        var version = BinaryCodec.ReadInt32(stream);
        if (version != expectedVersion)
            throw new InvalidDataException($"Unsupported package artifact format version: {version}.");
    }
}
