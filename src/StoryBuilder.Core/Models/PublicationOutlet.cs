namespace StoryBuilder.Core.Models;

/// <summary>
/// Stable identity for an authored publication source. This describes who issues artifacts;
/// it does not model a channel, delivery route, subscriber, or recipient.
/// </summary>
public sealed record PublicationOutlet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string ArtifactType { get; init; } = "Publication";
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
