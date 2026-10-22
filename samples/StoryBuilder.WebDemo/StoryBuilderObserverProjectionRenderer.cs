using System.Globalization;
using StoryBuilder.Core.Models;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace StoryBuilder.WebDemo;

/// <summary>Renders an author-inspectable GUI.SVG view of an observer-safe knowledge projection.</summary>
public sealed class StoryBuilderObserverProjectionRenderer
{
    private readonly SvgGuiRenderer renderer = new();

    public string Render(StoryWorkspace workspace, ObserverKnowledgeProjection projection)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(projection);

        var eventsById = workspace.Events.ToDictionary(item => item.Id);
        var maxClaims = projection.Events.Select(item => item.PermittedClaims.Count).DefaultIfEmpty(0).Max();
        var width = Math.Max(100, 36 + maxClaims * 30);
        var height = Math.Max(30, 15 + projection.Events.Count * 10);
        var root = GuiBuilder.Create("svg-scene", "storybuilder-observer-projection")
            .Text("Observer-safe knowledge projection")
            .Property("viewBox", $"0 0 {width} {height}")
            .Property("aria-label", "Accepted observer knowledge only, grouped by world event")
            .Property("description", "Author inspection labels may resolve event titles from the workspace. The projection contract itself contains event IDs and accepted claims, not unrestricted world-event titles or summaries.");

        root.Child("svg-rect", "observer-projection-background", node => node
            .Property("x", "0").Property("y", "0")
            .Property("width", width.ToString(CultureInfo.InvariantCulture))
            .Property("height", height.ToString(CultureInfo.InvariantCulture))
            .Property("fill", "#10141b"));
        root.Child("svg-text", "observer-projection-title", node => node
            .Text("ACCEPTED KNOWLEDGE · OBSERVER VIEW")
            .Property("x", "4").Property("y", "6")
            .Property("font-size", "2.5").Property("font-weight", "700").Property("fill", "#edf0f5"));

        if (projection.Events.Count == 0)
        {
            root.Child("svg-text", "observer-projection-empty", node => node
                .Text("No accepted knowledge is available at this narration anchor.")
                .Property("x", "4").Property("y", "18")
                .Property("font-size", "1.8").Property("fill", "#a8b2c2"));
            return renderer.Render(root.Build(), 1000, (int)(height * 10));
        }

        for (var eventIndex = 0; eventIndex < projection.Events.Count; eventIndex++)
        {
            var projectedEvent = projection.Events[eventIndex];
            var y = 12 + eventIndex * 10;
            var title = eventsById.TryGetValue(projectedEvent.EventId, out var storyEvent)
                ? storyEvent.Title
                : $"Missing event {projectedEvent.EventId}";
            root.Child("svg-rect", $"observer-event-{projectedEvent.EventId:N}", node => node
                .Property("x", "4").Property("y", y.ToString(CultureInfo.InvariantCulture))
                .Property("width", "26").Property("height", "7").Property("rx", "0.7")
                .Property("fill", "#202f43").Property("stroke", "#8bb7e8").Property("stroke-width", "0.25"));
            root.Child("svg-text", $"observer-event-label-{projectedEvent.EventId:N}", node => node
                .Text(Shorten(title, 23))
                .Property("x", "4.8").Property("y", (y + 4.2).ToString(CultureInfo.InvariantCulture))
                .Property("font-size", "1.5").Property("fill", "#cfe4ff"));

            for (var claimIndex = 0; claimIndex < projectedEvent.PermittedClaims.Count; claimIndex++)
            {
                var claim = projectedEvent.PermittedClaims[claimIndex];
                var x = 32 + claimIndex * 30;
                root.Child("svg-rect", $"observer-claim-{projectedEvent.EventId:N}-{claim.KnowledgeRecordId:N}-{claim.Mode}", node => node
                    .Property("x", x.ToString(CultureInfo.InvariantCulture))
                    .Property("y", y.ToString(CultureInfo.InvariantCulture))
                    .Property("width", "29").Property("height", "7").Property("rx", "0.7")
                    .Property("fill", ClaimFill(claim.Mode)).Property("stroke", "#8494a8").Property("stroke-width", "0.2"));
                root.Child("svg-text", $"observer-claim-mode-{projectedEvent.EventId:N}-{claim.KnowledgeRecordId:N}-{claim.Mode}", node => node
                    .Text(ModeLabel(claim.Mode))
                    .Property("x", (x + 0.6).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 2.5).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.25").Property("font-weight", "700").Property("fill", "#f2f4f8"));
                var note = string.IsNullOrWhiteSpace(claim.Note) ? "(no note recorded)" : claim.Note;
                root.Child("svg-text", $"observer-claim-note-{projectedEvent.EventId:N}-{claim.KnowledgeRecordId:N}-{claim.Mode}", node => node
                    .Text(Shorten(note, 28))
                    .Property("x", (x + 0.6).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 5.3).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.15").Property("fill", "#d7e0ec"));
            }
        }

        return renderer.Render(root.Build(), 1000, (int)(height * 10));
    }

    private static string ModeLabel(NarrationClaimMode mode) => mode switch
    {
        NarrationClaimMode.DirectFact => "DIRECT FACT",
        NarrationClaimMode.AttributedReport => "ATTRIBUTED REPORT",
        NarrationClaimMode.Inference => "INFERENCE",
        NarrationClaimMode.Belief => "BELIEF",
        _ => mode.ToString().ToUpperInvariant()
    };

    private static string ClaimFill(NarrationClaimMode mode) => mode switch
    {
        NarrationClaimMode.DirectFact => "#202f43",
        NarrationClaimMode.AttributedReport => "#3b3020",
        NarrationClaimMode.Inference => "#24372c",
        NarrationClaimMode.Belief => "#3d2940",
        _ => "#28303b"
    };

    private static string Shorten(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..Math.Max(1, maxLength - 1)] + "…";
}
