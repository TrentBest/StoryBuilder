using System.Globalization;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace StoryBuilder.WebDemo;

/// <summary>Builds a GUI.SVG view of timeline identities and declared parent/branch relationships.</summary>
public sealed class StoryBuilderTimelineRenderer
{
    private readonly SvgGuiRenderer renderer = new();

    public string Render(StoryBuilderSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        var timelines = session.Workspace.Timelines;
        var depthById = new Dictionary<Guid, int>();
        int Depth(TimelineDefinition timeline, HashSet<Guid> visiting)
        {
            if (depthById.TryGetValue(timeline.Id, out var cached))
                return cached;
            if (timeline.ParentTimelineId is not Guid parentId)
                return depthById[timeline.Id] = 0;
            if (!visiting.Add(timeline.Id))
                return depthById[timeline.Id] = 0;
            var parent = timelines.FirstOrDefault(item => item.Id == parentId);
            var depth = parent is null ? 0 : Depth(parent, visiting) + 1;
            visiting.Remove(timeline.Id);
            return depthById[timeline.Id] = depth;
        }

        foreach (var timeline in timelines)
            Depth(timeline, []);

        var maxDepth = depthById.Values.DefaultIfEmpty(0).Max();
        var maxPlanningPoints = timelines.Select(timeline => session.Workspace.PlanningPoints.Count(point => point.TimelineId == timeline.Id)).DefaultIfEmpty(0).Max();
        var maxEvents = timelines.Select(timeline => session.Workspace.Events.Count(item => item.TimelineId == timeline.Id)).DefaultIfEmpty(0).Max();
        var maxMarkers = maxPlanningPoints + maxEvents;
        var width = Math.Max(100, (maxDepth + 1) * 32 + 8 + maxMarkers * 17);
        var height = Math.Max(36, timelines.Count * 15 + 16);
        var root = GuiBuilder.Create("svg-scene", "storybuilder-timeline-map")
            .Text("StoryBuilder timeline constellation")
            .Property("viewBox", $"0 0 {width} {height}")
            .Property("aria-label", "Timeline definitions and parent branch relationships")
            .Property("description", "A domain-derived view. Connectors represent declared parent timeline identities, not measured time or causality.");

        root.Child("svg-rect", "timeline-background", node => node
            .Property("x", "0").Property("y", "0").Property("width", width.ToString(CultureInfo.InvariantCulture))
            .Property("height", height.ToString(CultureInfo.InvariantCulture)).Property("fill", "#10141b"));
        root.Child("svg-text", "timeline-title", node => node
            .Text("TIMELINE CONSTELLATION")
            .Property("x", "4").Property("y", "6")
            .Property("font-size", "2.8").Property("font-weight", "700").Property("fill", "#edf0f5"));

        if (timelines.Count == 0)
        {
            root.Child("svg-text", "timeline-empty", node => node
                .Text("Create a root timeline to begin.")
                .Property("x", "4").Property("y", "18")
                .Property("font-size", "2").Property("fill", "#a8b2c2"));
            return renderer.Render(root.Build(), 1000, (int)(height * 10));
        }

        var rowById = timelines.Select((timeline, index) => (timeline.Id, Y: 13 + index * 15))
            .ToDictionary(item => item.Id, item => item.Y);

        foreach (var timeline in timelines)
        {
            var x = 4 + depthById[timeline.Id] * 32;
            var y = rowById[timeline.Id];
            if (timeline.ParentTimelineId is Guid parentId && rowById.TryGetValue(parentId, out var parentY))
            {
                var parentX = 4 + depthById.GetValueOrDefault(parentId) * 32;
                root.Child("svg-rect", $"branch-v-{timeline.Id:N}", node => node
                    .Property("x", (parentX + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("y", Math.Min(parentY, y).ToString(CultureInfo.InvariantCulture))
                    .Property("width", "0.25")
                    .Property("height", Math.Max(0.5, Math.Abs(y - parentY)).ToString(CultureInfo.InvariantCulture))
                    .Property("fill", "#6b88a8"));
                root.Child("svg-rect", $"branch-h-{timeline.Id:N}", node => node
                    .Property("x", (parentX + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 2).ToString(CultureInfo.InvariantCulture))
                    .Property("width", Math.Max(0.5, x - parentX - 2).ToString(CultureInfo.InvariantCulture))
                    .Property("height", "0.25").Property("fill", "#6b88a8"));
            }

            var label = string.IsNullOrWhiteSpace(timeline.Name) ? "Untitled timeline" : timeline.Name;
            root.Child("svg-rect", $"timeline-card-{timeline.Id:N}", node => node
                .Property("x", x.ToString(CultureInfo.InvariantCulture))
                .Property("y", y.ToString(CultureInfo.InvariantCulture))
                .Property("width", "27").Property("height", "5.5").Property("rx", "0.7")
                .Property("fill", timeline.IsBranch ? "#263329" : "#202a39")
                .Property("stroke", timeline.IsBranch ? "#a9d8c8" : "#8bb7e8")
                .Property("stroke-width", "0.25"));
            root.Child("svg-text", $"timeline-label-{timeline.Id:N}", node => node
                .Text(Shorten(label, 24))
                .Property("x", (x + 1).ToString(CultureInfo.InvariantCulture))
                .Property("y", (y + 3.4).ToString(CultureInfo.InvariantCulture))
                .Property("font-size", "1.5").Property("fill", "#edf0f5"));

            var points = session.Workspace.PlanningPoints.Where(point => point.TimelineId == timeline.Id).ToList();
            for (var index = 0; index < points.Count; index++)
            {
                var point = points[index];
                var pointX = x + 28 + index * 17;
                root.Child("svg-rect", $"planning-point-card-{point.Id:N}", node => node
                    .Property("x", pointX.ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 0.5).ToString(CultureInfo.InvariantCulture))
                    .Property("width", "15.5").Property("height", "4.5").Property("rx", "0.7")
                    .Property("fill", "#3b3020").Property("stroke", "#e6bd72").Property("stroke-width", "0.25"));
                root.Child("svg-text", $"planning-point-label-{point.Id:N}", node => node
                    .Text(Shorten(point.Label, 12))
                    .Property("x", (pointX + 0.7).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 3.2).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.35").Property("fill", "#ffe0a1"));
            }

            var events = session.Workspace.Events.Where(item => item.TimelineId == timeline.Id).ToList();
            for (var index = 0; index < events.Count; index++)
            {
                var storyEvent = events[index];
                var eventX = x + 28 + (points.Count + index) * 17;
                root.Child("svg-rect", $"world-event-card-{storyEvent.Id:N}", node => node
                    .Property("x", eventX.ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 0.5).ToString(CultureInfo.InvariantCulture))
                    .Property("width", "15.5").Property("height", "4.5").Property("rx", "0.7")
                    .Property("fill", "#202f43").Property("stroke", "#8bb7e8").Property("stroke-width", "0.25"));
                root.Child("svg-text", $"world-event-label-{storyEvent.Id:N}", node => node
                    .Text(Shorten(storyEvent.Title, 12))
                    .Property("x", (eventX + 0.7).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (y + 3.2).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.35").Property("fill", "#cfe4ff"));
            }
        }

        return renderer.Render(root.Build(), 1000, (int)(height * 10));
    }

    private static string Shorten(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..Math.Max(1, maxLength - 1)] + "…";
}
