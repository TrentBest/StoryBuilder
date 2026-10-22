using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class CompositionPackageValidatorTests
{
    [Fact]
    public void ValidResolvedPackagePassesWithoutArbitration()
    {
        var packageId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var contentHash = new string('a', 64);
        var configHash = new string('b', 64);
        var manifest = new CompositionPackageManifest
        {
            PackageId = packageId,
            WorkspaceId = workspaceId,
            ContentRevision = "revision-17",
            ContentSha256 = contentHash,
            ConfigurationSha256 = configHash,
            ArbitrationRecordId = Guid.NewGuid(),
            OutputProfile = "book",
            Bundles = [new MicroBundleRequirement { BundleId = 42, Version = "1.2.0", RequiredCapabilities = ["book.render"] }]
        };
        var configuration = new CompositionPackageConfiguration
        {
            PackageId = packageId,
            WorkspaceId = workspaceId,
            ContentRevision = "revision-17",
            OutputProfile = "book",
            Bundles = [new MicroBundleRuntimeConfiguration
            {
                BundleId = 42,
                BundleVersion = "1.2.0",
                Settings = [new MicroBundleSetting { Key = "page-size", ValueType = "string", Value = "trade" }]
            }]
        };

        var errors = new CompositionPackageValidator().Validate(
            manifest, configuration, contentHash, configHash, [42UL], ["book.render"]);

        Assert.Empty(errors);
    }

    [Fact]
    public void RejectsHashRevisionAndRequiredCapabilityMismatch()
    {
        var manifest = new CompositionPackageManifest
        {
            WorkspaceId = Guid.NewGuid(),
            ContentRevision = "revision-a",
            ContentSha256 = new string('a', 64),
            ConfigurationSha256 = new string('b', 64),
            ArbitrationRecordId = Guid.NewGuid(),
            Bundles = [new MicroBundleRequirement { BundleId = 42, Version = "1.0", RequiredCapabilities = ["book.render"] }]
        };
        var configuration = new CompositionPackageConfiguration
        {
            PackageId = manifest.PackageId,
            WorkspaceId = manifest.WorkspaceId,
            ContentRevision = "revision-b",
            OutputProfile = manifest.OutputProfile,
            Bundles = [new MicroBundleRuntimeConfiguration { BundleId = 42, BundleVersion = "1.0" }]
        };

        var errors = new CompositionPackageValidator().Validate(
            manifest, configuration, new string('c', 64), new string('b', 64), [42UL], []);

        Assert.Contains(errors, error => error.Contains("content revisions", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("content payload", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("capability", StringComparison.Ordinal));
    }

    [Fact]
    public void RejectsDuplicateBundleIdsAndUnselectedConfiguration()
    {
        var manifest = new CompositionPackageManifest
        {
            WorkspaceId = Guid.NewGuid(),
            ContentRevision = "r1",
            ContentSha256 = new string('a', 64),
            ConfigurationSha256 = new string('b', 64),
            ArbitrationRecordId = Guid.NewGuid(),
            Bundles =
            [
                new MicroBundleRequirement { BundleId = 7, Version = "1" },
                new MicroBundleRequirement { BundleId = 7, Version = "1" }
            ]
        };
        var configuration = new CompositionPackageConfiguration
        {
            PackageId = manifest.PackageId,
            WorkspaceId = manifest.WorkspaceId,
            ContentRevision = "r1",
            OutputProfile = manifest.OutputProfile,
            Bundles = [new MicroBundleRuntimeConfiguration { BundleId = 9, BundleVersion = "1" }]
        };

        var errors = new CompositionPackageValidator().Validate(
            manifest, configuration, manifest.ContentSha256, manifest.ConfigurationSha256, [7UL, 9UL], []);

        Assert.Contains(errors, error => error.Contains("repeats ID", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("unselected bundle", StringComparison.Ordinal));
    }
}
