namespace StoryBuilder.Core.Models;

/// <summary>Whether a record is still being drafted, proposed, accepted, or retired.</summary>
public enum CanonStatus
{
    Draft,
    Proposed,
    Accepted,
    Rejected,
    Deprecated
}

/// <summary>A character record that can start small and be enriched over time.</summary>
public sealed record CharacterProfile
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Unnamed character";
    public string Role { get; init; } = "Unspecified";
    public string Description { get; init; } = string.Empty;
    public string? Pronouns { get; init; }
    public string? Species { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
    public List<CharacterTrait> Traits { get; init; } = [];
    public List<CharacterRelationship> Relationships { get; init; } = [];
    public Dictionary<string, string> Notes { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Returns an updated copy; no field is required beyond a stable identity.</summary>
    public CharacterProfile Rename(string name) =>
        this with { Name = string.IsNullOrWhiteSpace(name) ? "Unnamed character" : name.Trim(), UpdatedAtUtc = DateTimeOffset.UtcNow };

    public CharacterProfile WithDescription(string? description) =>
        this with { Description = description?.Trim() ?? string.Empty, UpdatedAtUtc = DateTimeOffset.UtcNow };

    public CharacterProfile AddTrait(CharacterTrait trait) =>
        this with { Traits = [.. Traits, trait], UpdatedAtUtc = DateTimeOffset.UtcNow };

    public CharacterProfile RemoveTrait(Guid traitId) =>
        this with { Traits = Traits.Where(t => t.Id != traitId).ToList(), UpdatedAtUtc = DateTimeOffset.UtcNow };
}

/// <summary>A trait expressed with the author's chosen surface wording.</summary>
public sealed record CharacterTrait
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Term { get; init; } = string.Empty;
    public string Category { get; init; } = "Trait";
    /// <summary>Optional relative emphasis from 0 (subtle) to 1 (defining).</summary>
    public double Intensity { get; init; } = 0.5;
    public string? Notes { get; init; }
    /// <summary>Who described the character: the character, narrator, another person, or the author.</summary>
    public string Attribution { get; init; } = "Author";
    public CanonStatus Status { get; init; } = CanonStatus.Draft;
}

/// <summary>A named relationship between two characters.</summary>
public sealed record CharacterRelationship
{
    public Guid TargetCharacterId { get; init; }
    public string Label { get; init; } = "knows";
    public string? Notes { get; init; }
    public CanonStatus Status { get; init; } = CanonStatus.Proposed;
}
