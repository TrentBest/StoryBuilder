namespace StoryBuilder.Core.Models;

/// <summary>Broad category used to organize searchable story vocabulary.</summary>
public enum VocabularyCategory
{
    Adjective,
    Noun,
    Verb,
    Phrase,
    Other
}

/// <summary>An occurrence of a word in a specific authored context.</summary>
public sealed record WordOccurrence
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string SurfaceForm { get; init; } = string.Empty;
    public string CanonicalTerm { get; init; } = string.Empty;
    public VocabularyCategory Category { get; init; } = VocabularyCategory.Other;
    public Guid TargetId { get; init; }
    public string TargetLabel { get; init; } = string.Empty;
    public string Field { get; init; } = string.Empty;
    public int StartIndex { get; init; }
    public int Length { get; init; }
    public string? Context { get; init; }
    public string Attribution { get; init; } = "Author";
    public string? SourceId { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public DateTimeOffset RecordedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>A canonical vocabulary concept and its known alternate spellings or synonyms.</summary>
public sealed record VocabularyConcept
{
    public string CanonicalTerm { get; init; } = string.Empty;
    public VocabularyCategory Category { get; init; } = VocabularyCategory.Other;
    public List<string> Synonyms { get; init; } = [];
}

/// <summary>A domain-wide summary that retains the original wording and attribution of each use.</summary>
public sealed record VocabularyUsage
{
    public string CanonicalTerm { get; init; } = string.Empty;
    public int OccurrenceCount { get; init; }
    public int DistinctTargetCount { get; init; }
    public IReadOnlyList<WordOccurrence> Occurrences { get; init; } = Array.Empty<WordOccurrence>();
}
