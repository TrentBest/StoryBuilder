using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>A lightweight starting pattern; suggestions are never silently added as character facts.</summary>
public sealed record CharacterTemplate
{
    public string Name { get; init; } = "Unspecified";
    public string Description { get; init; } = "Start with only what you know; details can be added later.";
    public IReadOnlyList<string> SuggestedPrompts { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SuggestedTraits { get; init; } = Array.Empty<string>();
}

/// <summary>Creates editable character drafts with optional, non-binding guidance.</summary>
public sealed class CharacterBuilder
{
    private static readonly IReadOnlyList<CharacterTemplate> Templates =
    [
        new CharacterTemplate
        {
            Name = "Unspecified",
            SuggestedPrompts = ["What do others notice first?", "What does this character want?", "What are they hiding?"],
            SuggestedTraits = ["curious", "guarded", "patient"]
        },
        new CharacterTemplate
        {
            Name = "Adventurer",
            SuggestedPrompts = ["What keeps them moving?", "What line will they not cross?", "What do they fear losing?"],
            SuggestedTraits = ["resourceful", "reckless", "observant"]
        },
        new CharacterTemplate
        {
            Name = "Investigator",
            SuggestedPrompts = ["What detail do they notice?", "Who do they trust?", "What evidence could change their mind?"],
            SuggestedTraits = ["methodical", "skeptical", "persistent"]
        },
        new CharacterTemplate
        {
            Name = "Antagonist",
            SuggestedPrompts = ["What do they believe is justified?", "What do they want?", "What would make them reconsider?"],
            SuggestedTraits = ["unyielding", "charming", "calculating"]
        },
        new CharacterTemplate
        {
            Name = "Witness",
            SuggestedPrompts = ["What did they actually observe?", "What do they assume?", "What might they have missed?"],
            SuggestedTraits = ["anxious", "precise", "conflicted"]
        }
    ];

    public IReadOnlyList<CharacterTemplate> GetTemplates() => Templates;

    /// <summary>Creates a draft without requiring a biography or accepting any suggested trait as fact.</summary>
    public CharacterProfile Create(string? name = null, string? templateName = null)
    {
        var template = FindTemplate(templateName);
        return new CharacterProfile
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Unnamed character" : name.Trim(),
            Role = template.Name == "Unspecified" ? "Unspecified" : template.Name,
            Description = string.Empty,
            Status = CanonStatus.Draft
        };
    }

    public CharacterTemplate FindTemplate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Templates[0];

        return Templates.FirstOrDefault(t => string.Equals(t.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? Templates[0];
    }
}
