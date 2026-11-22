using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Builds a validated, immutable handoff from a manuscript and its explicit source identity map.
/// No latest visual revision is inferred: every composed section must have one unambiguous,
/// explicitly approved review for the exact revision being exported.
/// </summary>
public static class PublicationSnapshotExporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Exports directly from a manifest import result so source provenance and the identity map
    /// cannot be accidentally mixed with values from another import.
    /// </summary>
    public static PublicationSnapshot Export(
        CompositionManifestImportResult import,
        Guid manuscriptId)
    {
        ArgumentNullException.ThrowIfNull(import);
        return Export(
            import.Session,
            manuscriptId,
            import.SourcePath,
            import.SourceBlobSha,
            import.SourceSectionIds);
    }

    public static PublicationSnapshot Export(
        StoryBuilderSession session,
        Guid manuscriptId,
        string sourceManifestPath,
        string sourceManifestBlobSha,
        IReadOnlyDictionary<string, Guid> sourceSectionIds)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceManifestPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceManifestBlobSha);
        ArgumentNullException.ThrowIfNull(sourceSectionIds);

        if (!IsGitBlobSha1(sourceManifestBlobSha))
            throw new InvalidDataException("The source manifest blob SHA must be a 40-character hexadecimal Git SHA-1.");

        if (sourceSectionIds.Count == 0 ||
            sourceSectionIds.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value == Guid.Empty))
            throw new InvalidDataException("The source identity map must contain non-empty source IDs and section IDs.");
        if (sourceSectionIds.Keys.Distinct(StringComparer.Ordinal).Count() != sourceSectionIds.Count)
            throw new InvalidDataException("The source identity map contains duplicate source IDs.");
        if (sourceSectionIds.Values.Distinct().Count() != sourceSectionIds.Count)
            throw new InvalidDataException("The source identity map maps multiple source IDs to the same manuscript section.");

        var manuscript = session.Workspace.Manuscripts.SingleOrDefault(item => item.Id == manuscriptId)
            ?? throw new InvalidDataException($"Manuscript '{manuscriptId}' does not exist.");
        var sections = manuscript.Sections.OrderBy(item => item.Sequence).ToArray();
        if (sections.Length != sourceSectionIds.Count)
            throw new InvalidDataException("Every manuscript section must have exactly one source identity mapping.");
        if (sections.Any(item => item.Sequence < 1) ||
            sections.Select(item => item.Sequence).Distinct().Count() != sections.Length)
            throw new InvalidDataException("Manuscript section order must use unique positive sequence numbers.");

        var sectionIds = sections.Select(item => item.Id).ToHashSet();
        if (sourceSectionIds.Values.Any(id => !sectionIds.Contains(id)) ||
            sectionIds.Any(id => !sourceSectionIds.Values.Contains(id)))
            throw new InvalidDataException("The source identity map does not exactly cover this manuscript.");

        var sourceBySection = sourceSectionIds.ToDictionary(pair => pair.Value, pair => pair.Key);
        var exportedSections = new List<PublicationSection>(sections.Length);
        foreach (var section in sections)
        {
            var compositions = session.Workspace.PageCompositions
                .Where(item => item.ManuscriptSectionId == section.Id)
                .ToArray();

            if (section.Kind == ManuscriptSectionKind.Scene)
            {
                if (compositions.Length != 1)
                    throw new InvalidDataException($"Story section '{sourceBySection[section.Id]}' must have exactly one page composition.");
            }
            else if (compositions.Length != 0)
            {
                throw new InvalidDataException($"Non-story section '{sourceBySection[section.Id]}' cannot carry a page composition in this publication contract.");
            }

            var composition = compositions.Length == 0
                ? null
                : BuildComposition(session, section, compositions[0]);

            exportedSections.Add(new PublicationSection
            {
                SourceId = sourceBySection[section.Id],
                SectionId = section.Id,
                Sequence = section.Sequence,
                Kind = section.Kind.ToString(),
                Title = section.Title,
                Body = section.Body,
                BodySha256 = Sha256(Encoding.UTF8.GetBytes(section.Body)),
                Composition = composition
            });
        }

        var snapshot = new PublicationSnapshot
        {
            Source = new PublicationSourceProvenance
            {
                ManifestPath = sourceManifestPath,
                ManifestBlobSha = sourceManifestBlobSha,
                SourceSectionIds = sourceSectionIds.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
            },
            Manuscript = new PublicationManuscript
            {
                Id = manuscript.Id,
                Title = manuscript.Title,
                Subtitle = manuscript.Subtitle,
                Author = manuscript.Author
            },
            Sections = exportedSections.AsReadOnly()
        };

        var hashInput = JsonSerializer.SerializeToUtf8Bytes(snapshot with { ContentSha256 = string.Empty }, JsonOptions);
        return snapshot with { ContentSha256 = Sha256(hashInput) };
    }

    public static string Serialize(PublicationSnapshot snapshot)
    {
        ValidateSnapshot(snapshot);
        return JsonSerializer.Serialize(snapshot, JsonOptions);
    }

    /// <summary>
    /// Reads a publication handoff and verifies its schema, content digest, section digests,
    /// and source-to-section identity map before a consumer can use it.
    /// </summary>
    public static PublicationSnapshot Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var snapshot = JsonSerializer.Deserialize<PublicationSnapshot>(json, JsonOptions)
            ?? throw new InvalidDataException("Publication snapshot document is empty.");
        ValidateSnapshot(snapshot);
        return snapshot;
    }

    private static void ValidateSnapshot(PublicationSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.SchemaVersion != 1)
            throw new InvalidDataException($"Unsupported publication snapshot schema version '{snapshot.SchemaVersion}'.");
        if (snapshot.Source is null || snapshot.Manuscript is null || snapshot.Sections is null)
            throw new InvalidDataException("Publication snapshot is missing source, manuscript, or section data.");
        if (snapshot.Id == Guid.Empty || snapshot.Manuscript.Id == Guid.Empty)
            throw new InvalidDataException("Publication snapshot and manuscript IDs must be non-empty.");
        if (string.IsNullOrWhiteSpace(snapshot.Source.ManifestPath) ||
            !IsGitBlobSha1(snapshot.Source.ManifestBlobSha))
            throw new InvalidDataException("Publication snapshot source provenance is incomplete or invalid.");
        if (snapshot.Source.SourceSectionIds is null)
            throw new InvalidDataException("Publication snapshot source identity map is missing.");
        if (snapshot.Sections.Count == 0 ||
            snapshot.Sections.Any(section => section is null ||
                string.IsNullOrWhiteSpace(section.SourceId) ||
                section.SectionId == Guid.Empty ||
                section.Sequence < 1 ||
                string.IsNullOrWhiteSpace(section.Kind) ||
                section.Body is null ||
                !IsSha256(section.BodySha256) ||
                !string.Equals(Sha256(Encoding.UTF8.GetBytes(section.Body)), section.BodySha256, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("Publication snapshot contains an invalid section or section body digest.");
        if (snapshot.Sections.Any(section => section.Composition is not null &&
            (section.Composition.VisualAsset is null ||
             section.Composition.TextRegion is null ||
             section.Composition.ImageRegion is null ||
             section.Composition.VisualAsset.Approval is null)))
            throw new InvalidDataException("Publication snapshot contains an incomplete page composition or visual approval.");
        if (snapshot.Sections.Select(section => section.Sequence).Distinct().Count() != snapshot.Sections.Count ||
            snapshot.Sections.Select(section => section.SectionId).Distinct().Count() != snapshot.Sections.Count ||
            snapshot.Sections.Select(section => section.SourceId).Distinct(StringComparer.Ordinal).Count() != snapshot.Sections.Count)
            throw new InvalidDataException("Publication snapshot section identities and sequence numbers must be unique.");
        if (snapshot.Sections.Any(section => section.Composition is null && section.Kind == ManuscriptSectionKind.Scene.ToString()) ||
            snapshot.Sections.Any(section => section.Composition is not null && section.Kind != ManuscriptSectionKind.Scene.ToString()))
            throw new InvalidDataException("Publication snapshot composition does not match its section kind.");

        var sourceMap = snapshot.Source.SourceSectionIds;
        if (sourceMap.Count != snapshot.Sections.Count ||
            sourceMap.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value == Guid.Empty) ||
            snapshot.Sections.Any(section => !sourceMap.TryGetValue(section.SourceId, out var id) || id != section.SectionId))
            throw new InvalidDataException("Publication snapshot source identity map does not exactly match its sections.");

        var expectedHash = Sha256(JsonSerializer.SerializeToUtf8Bytes(
            snapshot with { ContentSha256 = string.Empty }, JsonOptions));
        if (!IsSha256(snapshot.ContentSha256) ||
            !string.Equals(expectedHash, snapshot.ContentSha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Publication snapshot content hash does not match its contents.");
    }

    private static PublicationComposition BuildComposition(
        StoryBuilderSession session,
        ManuscriptSection section,
        PageComposition composition)
    {
        if (composition.VisualAssetId is not Guid assetId)
            throw new InvalidDataException($"Page composition '{composition.Id}' has no visual asset.");
        var asset = session.Workspace.VisualAssets.SingleOrDefault(item => item.Id == assetId)
            ?? throw new InvalidDataException($"Visual asset '{assetId}' does not exist.");
        if (asset.Kind != VisualAssetKind.PageIllustration || asset.ManuscriptSectionId != section.Id)
            throw new InvalidDataException($"Visual asset '{assetId}' is not the page illustration for section '{section.Id}'.");
        if (asset.Revisions.Count == 0)
            throw new InvalidDataException($"Visual asset '{assetId}' has no revisions.");
        if (asset.Revisions.Count != 1)
            throw new InvalidDataException(
                $"Visual asset '{assetId}' has {asset.Revisions.Count} revisions, but this page composition does not identify an exact revision.");

        var approvedRevision = asset.Revisions
            .Select(revision => new
            {
                Revision = revision,
                Reviews = session.Workspace.VisualAssetReviews
                    .Where(review => review.AssetId == asset.Id && review.RevisionId == revision.Id)
                    .ToArray()
            })
            .Where(item => item.Reviews.Length > 0)
            .Select(item => new
            {
                item.Revision,
                LatestAt = item.Reviews.Max(review => review.ReviewedAtUtc),
                LatestReviews = item.Reviews.Where(review => review.ReviewedAtUtc == item.Reviews.Max(candidate => candidate.ReviewedAtUtc)).ToArray()
            })
            .Where(item => item.LatestReviews.Length == 1 &&
                           item.LatestReviews[0].Decision == VisualAssetReviewDecision.Approved)
            .ToArray();

        if (approvedRevision.Length != 1)
            throw new InvalidDataException(
                $"Visual asset '{asset.Id}' must have exactly one revision with an unambiguous latest Approved review; found {approvedRevision.Length}.");

        var selected = approvedRevision[0];
        var revision = selected.Revision;
        var review = selected.LatestReviews[0];
        if (string.IsNullOrWhiteSpace(revision.AssetLocation) ||
            string.IsNullOrWhiteSpace(revision.MediaType) ||
            !IsSha256(revision.ContentSha256))
            throw new InvalidDataException($"Approved revision '{revision.Id}' is missing a durable location, media type, or valid SHA-256 hash.");
        if (string.IsNullOrWhiteSpace(review.Reviewer) || review.ReviewedAtUtc == default)
            throw new InvalidDataException($"Approval for revision '{revision.Id}' is missing reviewer or review time.");

        return new PublicationComposition
        {
            Id = composition.Id,
            Name = composition.Name,
            Orientation = composition.Orientation.ToString(),
            ImageFit = composition.ImageFit.ToString(),
            TextRegion = ToRegion(composition.TextRegion),
            ImageRegion = ToRegion(composition.ImageRegion),
            DiscoveryRegion = composition.DiscoveryRegion is null ? null : ToRegion(composition.DiscoveryRegion),
            FocalPoint = composition.FocalPoint,
            DiscoveryPrompt = composition.DiscoveryPrompt,
            DiscoveryHintDelaySeconds = composition.DiscoveryHintDelaySeconds,
            LayerOrder = composition.LayerOrder,
            VisualAsset = new PublicationVisualAsset
            {
                AssetId = asset.Id,
                RevisionId = revision.Id,
                RevisionNumber = revision.RevisionNumber,
                Location = revision.AssetLocation,
                MediaType = revision.MediaType,
                ContentSha256 = revision.ContentSha256!,
                Approval = new PublicationApproval
                {
                    ReviewId = review.Id,
                    Decision = review.Decision.ToString(),
                    Reviewer = review.Reviewer,
                    ReviewedAtUtc = review.ReviewedAtUtc,
                    Notes = review.Notes
                }
            }
        };
    }

    private static PublicationRegion ToRegion(PageCompositionRegion region) => new()
    {
        X = region.X,
        Y = region.Y,
        Width = region.Width,
        Height = region.Height
    };

    private static bool IsGitBlobSha1(string value) =>
        value.Length == 40 && value.All(Uri.IsHexDigit);

    private static bool IsSha256(string? value) =>
        value is { Length: 64 } && value.All(Uri.IsHexDigit);

    private static string Sha256(byte[] bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
}
