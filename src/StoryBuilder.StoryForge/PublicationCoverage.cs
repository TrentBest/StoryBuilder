namespace StoryBuilder.StoryForge;

/// <summary>Maps an authored unit to the interval of story time it presents.</summary>
public sealed record PublicationCoverage
{
    /// <summary>Creates a coverage record for a chapter, volume, episode, or other unit.</summary>
    public PublicationCoverage(string workId, string unitId, string unitKind, int sequence, StoryTimeRange storyRange)
    {
        if (string.IsNullOrWhiteSpace(workId)) throw new ArgumentException("Work id is required.", nameof(workId));
        if (string.IsNullOrWhiteSpace(unitId)) throw new ArgumentException("Unit id is required.", nameof(unitId));
        if (string.IsNullOrWhiteSpace(unitKind)) throw new ArgumentException("Unit kind is required.", nameof(unitKind));
        if (sequence < 0) throw new ArgumentOutOfRangeException(nameof(sequence), "Sequence must be zero or greater.");
        WorkId = workId;
        UnitId = unitId;
        UnitKind = unitKind;
        Sequence = sequence;
        StoryRange = storyRange ?? throw new ArgumentNullException(nameof(storyRange));
    }

    /// <summary>Gets the work identifier.</summary>
    public string WorkId { get; }
    /// <summary>Gets the authored unit identifier.</summary>
    public string UnitId { get; }
    /// <summary>Gets the kind of unit, such as chapter, book, or episode.</summary>
    public string UnitKind { get; }
    /// <summary>Gets the zero-based or project-defined publication sequence number.</summary>
    public int Sequence { get; }
    /// <summary>Gets the interval of story time covered by this unit.</summary>
    public StoryTimeRange StoryRange { get; }
}

/// <summary>An immutable published-edition snapshot with fixed identity and coverage.</summary>
public sealed record PublishedEdition
{
    /// <summary>Creates an edition snapshot. Later authoring should create a new snapshot.</summary>
    public PublishedEdition(string editionId, string workId, string version, string contentHash, IEnumerable<PublicationCoverage> coverage)
    {
        if (string.IsNullOrWhiteSpace(editionId)) throw new ArgumentException("Edition id is required.", nameof(editionId));
        if (string.IsNullOrWhiteSpace(workId)) throw new ArgumentException("Work id is required.", nameof(workId));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("Version is required.", nameof(version));
        if (string.IsNullOrWhiteSpace(contentHash)) throw new ArgumentException("Content hash is required.", nameof(contentHash));

        var snapshot = (coverage ?? throw new ArgumentNullException(nameof(coverage))).ToArray();
        if (snapshot.Length == 0)
            throw new ArgumentException("A published edition must cover at least one authored unit.", nameof(coverage));
        if (snapshot.Any(item => item is null))
            throw new ArgumentException("Publication coverage cannot contain null entries.", nameof(coverage));
        if (snapshot.Any(item => !string.Equals(item.WorkId, workId, StringComparison.Ordinal)))
            throw new ArgumentException("Every coverage record must belong to the edition's work.", nameof(coverage));
        if (snapshot.Select(item => item.UnitId).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Publication unit identifiers must be unique within an edition.", nameof(coverage));
        if (snapshot.Select(item => item.Sequence).Distinct().Count() != snapshot.Length)
            throw new ArgumentException("Publication sequence numbers must be unique within an edition.", nameof(coverage));

        EditionId = editionId;
        WorkId = workId;
        Version = version;
        ContentHash = contentHash;
        Coverage = Array.AsReadOnly(snapshot);
    }

    /// <summary>Gets the immutable edition identifier.</summary>
    public string EditionId { get; }
    /// <summary>Gets the work identifier.</summary>
    public string WorkId { get; }
    /// <summary>Gets the edition version label.</summary>
    public string Version { get; }
    /// <summary>Gets the content hash identifying the published bytes.</summary>
    public string ContentHash { get; }
    /// <summary>Gets a read-only snapshot of the edition's publication coverage.</summary>
    public IReadOnlyList<PublicationCoverage> Coverage { get; }
}
