using System.Globalization;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace StoryBuilder.WebDemo;

/// <summary>Builds a living SVG manifestation from the current StoryBuilder domain session.</summary>
public sealed class StoryBuilderLivingMapRenderer
{
    private readonly SvgGuiRenderer _renderer = new();

    public string Render(StoryBuilderSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var root = GuiBuilder.Create("svg-scene", "storybuilder-living-map")
            .Text("StoryBuilder living workspace map")
            .Property("viewBox", "0 0 100 90")
            .Property("aria-label", "A live view of StoryBuilder character records and vocabulary usage")
            .Property("description", "Character cards and the most-used words are generated from the current StoryBuilder workspace.");

        root.Child("svg-rect", "map-background", node => node
            .Property("x", "0").Property("y", "0").Property("width", "100").Property("height", "90")
            .Property("fill", "#10141b"));

        root.Child("svg-text", "map-title", node => node
            .Text("StoryBuilder · Living Workspace")
            .Property("x", "4").Property("y", "5.5")
            .Property("font-size", "3.2").Property("font-weight", "700").Property("fill", "#edf0f5"));

        root.Child("svg-text", "map-subtitle", node => node
            .Text($"{session.Workspace.Characters.Count} characters · {session.Workspace.Scenes.Count} scenes · {session.Workspace.HistoricalClaims.Count} historical claims · {session.Workspace.Campaigns.Count} campaigns")
            .Property("x", "4").Property("y", "9")
            .Property("font-size", "1.45").Property("fill", "#a8b2c2"));

        root.Child("svg-text", "cast-heading", node => node
            .Text("CASTING")
            .Property("x", "4").Property("y", "13")
            .Property("font-size", "1.6").Property("font-weight", "700").Property("fill", "#e7c48e"));

        var characters = session.Workspace.Characters.Take(6).ToArray();
        if (characters.Length == 0)
        {
            root.Child("svg-text", "empty-cast", node => node
                .Text("The casting room is empty. Create a character to begin.")
                .Property("x", "4").Property("y", "20")
                .Property("font-size", "1.8").Property("fill", "#a8b2c2"));
        }
        else
        {
            for (var i = 0; i < characters.Length; i++)
            {
                var character = characters[i];
                var x = 4 + (i % 2) * 48;
                var y = 15 + (i / 2) * 12;
                var cardId = $"character-card-{i}";
                var statusColor = character.Status switch
                {
                    CanonStatus.Accepted => "#a9d8c8",
                    CanonStatus.Proposed => "#e7c48e",
                    CanonStatus.Rejected => "#d88780",
                    CanonStatus.Deprecated => "#657080",
                    _ => "#8bb7e8"
                };
                var traits = string.Join(", ", character.Traits.Take(3).Select(t => t.Term));
                if (character.Traits.Count > 3)
                    traits += "…";

                root.Child("svg-rect", cardId, node => node
                    .Property("x", x.ToString(CultureInfo.InvariantCulture))
                    .Property("y", y.ToString(CultureInfo.InvariantCulture))
                    .Property("width", "44").Property("height", "10.5").Property("rx", "1")
                    .Property("fill", "#1a202b").Property("stroke", "#354153").Property("stroke-width", "0.25"));

                root.Child("svg-circle", $"{cardId}-status", node => node
                    .Property("cx", (x + 41).ToString(CultureInfo.InvariantCulture))
                    .Property("cy", (y + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("r", "0.8").Property("fill", statusColor));

                root.Child("svg-text", $"{cardId}-name", node => node
                    .Text(Shorten(character.Name, 28))
                    .Property("x", (x + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 3).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.8").Property("font-weight", "700").Property("fill", "#edf0f5"));

                root.Child("svg-text", $"{cardId}-role", node => node
                    .Text(Shorten($"{character.NarrativeRole} · {character.Role}", 34))
                    .Property("x", (x + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 5.4).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.25").Property("fill", "#a8b2c2"));

                root.Child("svg-text", $"{cardId}-traits", node => node
                    .Text(Shorten(string.IsNullOrWhiteSpace(traits) ? "No traits recorded yet" : traits, 42))
                    .Property("x", (x + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 8.4).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.25").Property("fill", "#a9d8c8"));
            }

            if (session.Workspace.Characters.Count > characters.Length)
            {
                root.Child("svg-text", "additional-cast", node => node
                    .Text($"+ {session.Workspace.Characters.Count - characters.Length} more character(s) in the workspace")
                    .Property("x", "4").Property("y", "52")
                    .Property("font-size", "1.25").Property("fill", "#a8b2c2"));
            }
        }

        root.Child("svg-text", "vocabulary-heading", node => node
            .Text("MOST-USED WORDS")
            .Property("x", "4").Property("y", "56")
            .Property("font-size", "1.6").Property("font-weight", "700").Property("fill", "#e7c48e"));

        var mostUsed = session.Vocabulary.GetMostUsed(7);
        var maxCount = Math.Max(1, mostUsed.Select(item => item.OccurrenceCount).DefaultIfEmpty(1).Max());
        if (mostUsed.Count == 0)
        {
            root.Child("svg-text", "empty-vocabulary", node => node
                .Text("Add character traits or scene text to populate the vocabulary view.")
                .Property("x", "4").Property("y", "61")
                .Property("font-size", "1.4").Property("fill", "#a8b2c2"));
        }
        else
        {
            for (var i = 0; i < mostUsed.Count; i++)
            {
                var usage = mostUsed[i];
                var y = 60 + i * 4;
                var barWidth = Math.Max(1.2, 34d * usage.OccurrenceCount / maxCount);
                var id = $"word-usage-{i}";
                root.Child("svg-text", $"{id}-label", node => node
                    .Text(Shorten(usage.CanonicalTerm, 22))
                    .Property("x", "4").Property("y", (y + 1.7).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.35").Property("fill", "#edf0f5"));
                root.Child("svg-rect", $"{id}-bar", node => node
                    .Property("x", "30").Property("y", (y + 0.4).ToString(CultureInfo.InvariantCulture))
                    .Property("width", barWidth.ToString("0.##", CultureInfo.InvariantCulture))
                    .Property("height", "1.6").Property("rx", "0.4").Property("fill", "#a9d8c8"));
                root.Child("svg-text", $"{id}-count", node => node
                    .Text($"{usage.OccurrenceCount} use(s) · {usage.DistinctTargetCount} target(s)")
                    .Property("x", "67").Property("y", (y + 1.7).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.2").Property("fill", "#a8b2c2"));
            }
        }

        return _renderer.Render(root.Build(), 1000, 900);
    }

    private static string Shorten(string value, int maximumLength) =>
        value.Length <= maximumLength ? value : value[..Math.Max(1, maximumLength - 1)] + "…";
}
