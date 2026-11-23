namespace StoryBuilder.Core.Models;

/// <summary>A structured, editable book manuscript. Publication freezes a snapshot, not this working draft.</summary>
public sealed record ManuscriptEdition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "Untitled book";
    public string Subtitle { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public List<ManuscriptSection> Sections { get; init; } = [];
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public ManuscriptEdition AddSection(ManuscriptSection section)
    {
        ArgumentNullException.ThrowIfNull(section);
        if (Sections.Any(existing => existing.Id == section.Id))
            throw new InvalidOperationException($"A manuscript section with ID '{section.Id}' already exists.");
        var nextSequence = Sections.Count == 0 ? 1 : Sections.Max(existing => existing.Sequence) + 1;
        var normalized = section with { Sequence = section.Sequence > 0 ? section.Sequence : nextSequence };
        return this with { Sections = [.. Sections, normalized], UpdatedAtUtc = DateTimeOffset.UtcNow };
    }

    public ManuscriptEdition ReplaceSection(ManuscriptSection section)
    {
        ArgumentNullException.ThrowIfNull(section);
        var index = Sections.FindIndex(existing => existing.Id == section.Id);
        if (index < 0)
            throw new KeyNotFoundException($"No manuscript section with ID '{section.Id}' exists.");
        var updated = Sections.ToList();
        updated[index] = section;
        return this with { Sections = updated, UpdatedAtUtc = DateTimeOffset.UtcNow };
    }
}

/// <summary>A manuscript unit with explicit reading order and optional world-record links.</summary>
public sealed record ManuscriptSection
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Sequence { get; init; }
    public ManuscriptSectionKind Kind { get; init; } = ManuscriptSectionKind.Chapter;
    public string Title { get; init; } = "Untitled section";
    public string Body { get; init; } = string.Empty;
    public List<Guid> SceneIds { get; init; } = [];
    public List<Guid> EventIds { get; init; } = [];

    /// <summary>Append-only acceptance history; edits to Body do not erase earlier accepted snapshots.</summary>
    public List<NarrationAcceptanceRecord> NarrationAcceptances { get; init; } = [];
}

/// <summary>An immutable snapshot of one author-approved narration proposal and its text provenance.</summary>
public sealed record NarrationAcceptanceRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid RequestId { get; init; }
    public Guid SceneId { get; init; }
    public string AcceptedText { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string? ProviderId { get; init; }
    public string? ModelId { get; init; }
    public DateTimeOffset AcceptedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<NarrationDraftSegment> Segments { get; init; } = Array.Empty<NarrationDraftSegment>();
    public IReadOnlyList<NarrationEventClaim> Claims { get; init; } = Array.Empty<NarrationEventClaim>();
}

/// <summary>Structural role of a section in the current manuscript.</summary>
public enum ManuscriptSectionKind
{
    FrontMatter,
    Chapter,
    Scene,
    Passage,
    BackMatter
}
