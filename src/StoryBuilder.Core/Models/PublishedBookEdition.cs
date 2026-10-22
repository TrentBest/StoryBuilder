namespace StoryBuilder.Core.Models;

/// <summary>
/// An immutable publication snapshot of a manuscript. Later corrections create a new record
/// linked to the prior edition; the published Markdown is never regenerated from a changed draft.
/// </summary>
public sealed record PublishedBookEdition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ManuscriptId { get; init; }
    public string Title { get; init; } = "Untitled book";
    public string Subtitle { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Markdown { get; init; } = string.Empty;
    public string ContentSha256 { get; init; } = string.Empty;
    public DateTimeOffset PublishedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset SourceUpdatedAtUtc { get; init; }
    public Guid? PreviousEditionId { get; init; }
    public string? CorrectionNotes { get; init; }
    public IReadOnlyList<Guid> SectionIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<Guid> SceneIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<Guid> EventIds { get; init; } = Array.Empty<Guid>();
}
