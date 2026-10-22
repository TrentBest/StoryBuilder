namespace StoryBuilder.Core.Models;

/// <summary>A deterministic digital book product rendered from one working manuscript, not a physical print layout.</summary>
public sealed record RenderedBookArtifact(
    Guid ManuscriptId,
    string Title,
    string Subtitle,
    string Author,
    string Markdown,
    int SectionCount,
    IReadOnlyList<Guid> ReferencedEventIds);
