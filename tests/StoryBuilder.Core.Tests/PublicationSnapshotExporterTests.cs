using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class PublicationSnapshotExporterTests
{
    private const string SourceManifestBlobSha = "685d275782b76a4c5bdd30d6ae69baeaea919993";

    [Fact]
    public void Export_freezes_source_order_composition_exact_revision_and_approval()
    {
        var fixture = CreateComposedStory();
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId,
            fixture.RevisionId,
            VisualAssetReviewDecision.Approved,
            "Author",
            "Approved for publication",
            DateTimeOffset.Parse("2026-11-21T18:00:00Z"));

        var snapshot = PublicationSnapshotExporter.Export(
            fixture.Session,
            fixture.ManuscriptId,
            "experiences/Lenny/CompositionManifest.json",
            SourceManifestBlobSha,
            fixture.SourceIds);

        Assert.Equal(1, snapshot.SchemaVersion);
        Assert.Equal("lenny-page-01", Assert.Single(snapshot.Sections).SourceId);
        Assert.Equal(fixture.SectionId, snapshot.Sections[0].SectionId);
        var composition = snapshot.Sections[0].Composition!;
        Assert.Equal(fixture.RevisionId, composition.VisualAsset.RevisionId);
        Assert.Equal("Author", composition.VisualAsset.Approval.Reviewer);

        var json = PublicationSnapshotExporter.Serialize(snapshot);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(snapshot.ContentSha256, document.RootElement.GetProperty("contentSha256").GetString());
    }

    [Fact]
    public void Export_fails_closed_when_source_manifest_blob_sha_is_not_a_git_blob_sha1()
    {
        var fixture = CreateComposedStory();

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", "source-sha", fixture.SourceIds));
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", new string('z', 40), fixture.SourceIds));
    }

    [Fact]
    public void Export_fails_closed_when_visual_revision_is_unreviewed()
    {
        var fixture = CreateComposedStory();

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds));
    }

    [Fact]
    public void Export_fails_closed_when_composition_cannot_select_between_multiple_revisions()
    {
        var fixture = CreateComposedStory();
        fixture.Session.AddVisualAssetRevision(fixture.AssetId, new VisualAssetRevision
        {
            RevisionNumber = 2,
            AssetLocation = "visuals/page-01-v2.png",
            MediaType = "image/png",
            ContentSha256 = new string('b', 64)
        });
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author");

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds));
    }

    [Fact]
    public void Export_fails_closed_when_latest_review_rejects_the_exact_revision()
    {
        var fixture = CreateComposedStory();
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author",
            reviewedAtUtc: DateTimeOffset.Parse("2026-11-21T18:00:00Z"));
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Rejected, "Author",
            reviewedAtUtc: DateTimeOffset.Parse("2026-11-21T19:00:00Z"));

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds));
    }

    [Fact]
    public void Export_fails_closed_when_latest_review_timestamp_is_ambiguous()
    {
        var fixture = CreateComposedStory();
        var reviewedAt = DateTimeOffset.Parse("2026-11-21T18:00:00Z");
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author",
            reviewedAtUtc: reviewedAt);
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Rejected, "Reviewer",
            reviewedAtUtc: reviewedAt);

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds));
    }

    [Fact]
    public void Deserialize_round_trips_a_valid_publication_snapshot()
    {
        var fixture = CreateComposedStory();
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author",
            reviewedAtUtc: DateTimeOffset.Parse("2026-11-21T18:00:00Z"));
        var snapshot = PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds);

        var restored = PublicationSnapshotExporter.Deserialize(PublicationSnapshotExporter.Serialize(snapshot));

        Assert.Equal(snapshot.ContentSha256, restored.ContentSha256);
        Assert.Equal(snapshot.Sections[0].Body, restored.Sections[0].Body);
        Assert.Equal(snapshot.Sections[0].Composition!.VisualAsset.RevisionId,
            restored.Sections[0].Composition!.VisualAsset.RevisionId);
    }

    [Fact]
    public void Deserialize_rejects_a_section_body_that_no_longer_matches_its_digest()
    {
        var fixture = CreateComposedStory();
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author",
            reviewedAtUtc: DateTimeOffset.Parse("2026-11-21T18:00:00Z"));
        var snapshot = PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds);
        var tampered = snapshot with
        {
            Sections = new[] { snapshot.Sections[0] with { Body = "Tampered after publication." } }
        };
        var json = JsonSerializer.Serialize(tampered, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Deserialize(json));
    }

    [Fact]
    public void Serialize_and_deserialize_reject_incomplete_snapshot_objects()
    {
        var fixture = CreateComposedStory();
        fixture.Session.ReviewVisualAssetRevision(
            fixture.AssetId, fixture.RevisionId, VisualAssetReviewDecision.Approved, "Author",
            reviewedAtUtc: DateTimeOffset.Parse("2026-11-21T18:00:00Z"));
        var snapshot = PublicationSnapshotExporter.Export(
            fixture.Session, fixture.ManuscriptId, "manifest.json", SourceManifestBlobSha, fixture.SourceIds);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Serialize(snapshot with { Source = null! }));
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Serialize(snapshot with { Manuscript = null! }));
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Serialize(snapshot with { Sections = null! }));
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Serialize(snapshot with
        {
            Sections = new[] { snapshot.Sections[0] with { Body = null! } }
        }));
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Serialize(snapshot with
        {
            Sections = new[]
            {
                snapshot.Sections[0] with
                {
                    Composition = snapshot.Sections[0].Composition! with
                    {
                        VisualAsset = snapshot.Sections[0].Composition!.VisualAsset with { Approval = null! }
                    }
                }
            }
        }));

        var malformedJson = JsonSerializer.Serialize(snapshot with { Source = null! }, jsonOptions);
        Assert.Throws<InvalidDataException>(() => PublicationSnapshotExporter.Deserialize(malformedJson));
    }

    [Fact]
    public void Export_uses_source_provenance_from_the_same_manifest_import_result()
    {
        var session = new StoryBuilderSession("Lenny");
        var manuscript = session.CreateManuscript("Lenny");
        manuscript = session.AddManuscriptSection(
            manuscript.Id, ManuscriptSectionKind.FrontMatter, "Dedication");
        var section = manuscript.Sections.Single();
        session.UpdateManuscriptSection(manuscript.Id, section.Id, section.Title, "For the ocean.");
        var import = new CompositionManifestImportResult(
            session,
            "experiences/Lenny/CompositionManifest.json",
            SourceManifestBlobSha,
            new Dictionary<string, Guid>(StringComparer.Ordinal) { ["lenny-dedication"] = section.Id });

        var snapshot = PublicationSnapshotExporter.Export(import, manuscript.Id);

        Assert.Equal(import.SourcePath, snapshot.Source.ManifestPath);
        Assert.Equal(import.SourceBlobSha, snapshot.Source.ManifestBlobSha);
        Assert.Equal(section.Id, snapshot.Source.SourceSectionIds["lenny-dedication"]);
        Assert.Equal("lenny-dedication", Assert.Single(snapshot.Sections).SourceId);
    }

    private static Fixture CreateComposedStory()
    {
        var session = new StoryBuilderSession("Lenny");
        var manuscript = session.CreateManuscript("Lenny");
        manuscript = session.AddManuscriptSection(
            manuscript.Id, ManuscriptSectionKind.Scene, "Page 01");
        var section = manuscript.Sections.Single();
        session.UpdateManuscriptSection(manuscript.Id, section.Id, section.Title, "A story page.");

        var asset = session.CreateVisualAsset(
            "Page 01 illustration", VisualAssetKind.PageIllustration,
            manuscriptSectionId: section.Id);
        var revision = new VisualAssetRevision
        {
            RevisionNumber = 1,
            AssetLocation = "visuals/page-01.png",
            MediaType = "image/png",
            ContentSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("image"))).ToLowerInvariant()
        };
        session.AddVisualAssetRevision(asset.Id, revision);
        session.Workspace.AddPageComposition(new PageComposition
        {
            ManuscriptSectionId = section.Id,
            VisualAssetId = asset.Id,
            Name = "Page 01 composition"
        });

        return new Fixture(
            session,
            manuscript.Id,
            section.Id,
            asset.Id,
            revision.Id,
            new Dictionary<string, Guid>(StringComparer.Ordinal) { ["lenny-page-01"] = section.Id });
    }

    private sealed record Fixture(
        StoryBuilderSession Session,
        Guid ManuscriptId,
        Guid SectionId,
        Guid AssetId,
        Guid RevisionId,
        IReadOnlyDictionary<string, Guid> SourceIds);
}
