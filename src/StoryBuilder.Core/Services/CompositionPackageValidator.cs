using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>Validates an already-resolved package against captured hashes and host capabilities. It never reruns Workshop arbitration.</summary>
public sealed class CompositionPackageValidator
{
    public IReadOnlyList<string> Validate(
        CompositionPackageManifest manifest,
        CompositionPackageConfiguration configuration,
        string actualContentSha256,
        string actualConfigurationSha256,
        IEnumerable<ulong> availableBundleIds,
        IEnumerable<string> availableCapabilities)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(availableBundleIds);
        ArgumentNullException.ThrowIfNull(availableCapabilities);

        var errors = new List<string>();
        if (manifest.SchemaVersion != CompositionPackageManifest.CurrentSchemaVersion)
            errors.Add($"Unsupported manifest schema version: {manifest.SchemaVersion}.");
        if (configuration.SchemaVersion != CompositionPackageConfiguration.CurrentSchemaVersion)
            errors.Add($"Unsupported configuration schema version: {configuration.SchemaVersion}.");
        if (manifest.PackageId == Guid.Empty || manifest.PackageId != configuration.PackageId)
            errors.Add("Manifest and configuration package identities do not match.");
        if (manifest.WorkspaceId == Guid.Empty || manifest.WorkspaceId != configuration.WorkspaceId)
            errors.Add("Manifest and configuration workspace identities do not match.");
        if (string.IsNullOrWhiteSpace(manifest.ContentRevision) ||
            !string.Equals(manifest.ContentRevision, configuration.ContentRevision, StringComparison.Ordinal))
            errors.Add("Manifest and configuration content revisions do not match.");
        if (string.IsNullOrWhiteSpace(manifest.OutputProfile) ||
            !string.Equals(manifest.OutputProfile, configuration.OutputProfile, StringComparison.Ordinal))
            errors.Add("Manifest and configuration output profiles do not match.");
        if (manifest.ArbitrationRecordId == Guid.Empty)
            errors.Add("The package does not identify the Workshop arbitration record.");
        if (!IsSha256(manifest.ContentSha256) ||
            !string.Equals(manifest.ContentSha256, actualContentSha256, StringComparison.OrdinalIgnoreCase))
            errors.Add("The content payload does not match the manifest SHA-256.");
        if (!IsSha256(manifest.ConfigurationSha256) ||
            !string.Equals(manifest.ConfigurationSha256, actualConfigurationSha256, StringComparison.OrdinalIgnoreCase))
            errors.Add("The configuration payload does not match the manifest SHA-256.");

        var requirements = manifest.Bundles ?? [];
        var configurations = configuration.Bundles ?? [];
        AddDuplicateErrors(requirements.Select(item => item.BundleId), "manifest bundle", errors);
        AddDuplicateErrors(configurations.Select(item => item.BundleId), "configured bundle", errors);

        var requirementIds = requirements.Select(item => item.BundleId).ToHashSet();
        foreach (var configured in configurations)
            if (!requirementIds.Contains(configured.BundleId))
                errors.Add($"Configuration contains unselected bundle '{configured.BundleId}'.");

        var configsById = configurations.GroupBy(item => item.BundleId)
            .ToDictionary(group => group.Key, group => group.First());
        var installedIds = availableBundleIds.ToHashSet();
        var capabilities = availableCapabilities.ToHashSet(StringComparer.Ordinal);
        foreach (var requirement in requirements)
        {
            if (requirement.BundleId == 0 || string.IsNullOrWhiteSpace(requirement.Version))
            {
                errors.Add("Every manifest bundle must have a non-zero ID and an explicit version.");
                continue;
            }

            if (!configsById.TryGetValue(requirement.BundleId, out var resolved))
            {
                if (requirement.Required)
                    errors.Add($"Required bundle '{requirement.BundleId}' has no resolved configuration.");
                continue;
            }

            if (!string.Equals(requirement.Version, resolved.BundleVersion, StringComparison.Ordinal))
                errors.Add($"Bundle '{requirement.BundleId}' version differs between manifest and configuration.");
            if (requirement.Required && !resolved.Enabled)
                errors.Add($"Required bundle '{requirement.BundleId}' is disabled in its configuration.");
            if (!installedIds.Contains(requirement.BundleId))
            {
                if (requirement.Required)
                    errors.Add($"Required bundle '{requirement.BundleId}' is not available in this host.");
                continue;
            }

            foreach (var capability in requirement.RequiredCapabilities ?? [])
                if (string.IsNullOrWhiteSpace(capability) || !capabilities.Contains(capability))
                    errors.Add($"Required capability '{capability}' for bundle '{requirement.BundleId}' is unavailable.");

            var settingKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var setting in resolved.Settings ?? [])
            {
                if (string.IsNullOrWhiteSpace(setting.Key) || string.IsNullOrWhiteSpace(setting.ValueType))
                    errors.Add($"Bundle '{requirement.BundleId}' has a setting without a key or value type.");
                else if (!settingKeys.Add(setting.Key))
                    errors.Add($"Bundle '{requirement.BundleId}' repeats setting key '{setting.Key}'.");
            }
        }

        return errors;
    }

    private static bool IsSha256(string? value) => value is { Length: 64 } && value.All(Uri.IsHexDigit);

    private static void AddDuplicateErrors(IEnumerable<ulong> ids, string kind, ICollection<string> errors)
    {
        foreach (var duplicate in ids.GroupBy(id => id).Where(group => group.Count() > 1).Select(group => group.Key))
            errors.Add($"The {kind} list repeats ID '{duplicate}'.");
    }
}
