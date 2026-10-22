using System.Globalization;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace StoryBuilder.WebDemo;

/// <summary>Builds a GUI.SVG view of timeline identities, planning points, events, and their book portrayals.</summary>
public sealed class StoryBuilderTimelineRenderer
{
    private readonly SvgGuiRenderer renderer = new();

    public string Render(StoryBuilderSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        var workspace = session.Workspace;
        var timelines = workspace.Timelines;
        var portrayalLinks = workspace.Manuscripts
            .OrderBy(manuscript => manuscript.Title, StringComparer.Ordinal)
            .SelectMany(manuscript => manuscript.Sections
                .OrderBy(section => section.Sequence)
                .SelectMany(section => section.EventIds.Select(eventId => new
                {
                    EventId = eventId,
                    ManuscriptId = manuscript.Id,
                    ManuscriptTitle = manuscript.Title,
                    Section = section
                })))
            .Where(link => workspace.Events.Any(storyEvent => storyEvent.Id == link.EventId))
            .ToList();

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
        var maxPlanningPoints = timelines.Select(timeline => workspace.PlanningPoints.Count(point => point.TimelineId == timeline.Id)).DefaultIfEmpty(0).Max();
        var maxEvents = timelines.Select(timeline => workspace.Events.Count(item => item.TimelineId == timeline.Id)).DefaultIfEmpty(0).Max();
        var maxMarkers = maxPlanningPoints + maxEvents;
        var width = Math.Max(100, (maxDepth + 1) * 32 + 8 + maxMarkers * 17);
        if (portrayalLinks.Count > 0)
            width += 64;
        var railHeight = portrayalLinks.Count == 0 ? 0 : 15 + portrayalLinks.Count * 8;
        var height = Math.Max(36, timelines.Count * 15 + 16 + railHeight);
        var root = GuiBuilder.Create("svg-scene", "storybuilder-timeline-map")
            .Text("StoryBuilder timeline constellation and book portrayal links")
            .Property("viewBox", $"0 0 {width} {height}")
            .Property("aria-label", "Timeline definitions, planning points, world events, and event-to-book-section portrayal links")
            .Property("description", "Parent connectors show timeline lineage. The portrayal rail connects a world event to manuscript sections that recount it; neither relationship implies measured world time.");

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
        var eventXById = new Dictionary<Guid, double>();
        var eventYById = new Dictionary<Guid, double>();

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

            var points = workspace.PlanningPoints.Where(point => point.TimelineId == timeline.Id).ToList();
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

            var events = workspace.Events.Where(item => item.TimelineId == timeline.Id).ToList();
            for (var index = 0; index < events.Count; index++)
            {
                var storyEvent = events[index];
                var eventX = x + 28 + (points.Count + index) * 17;
                eventXById[storyEvent.Id] = eventX;
                eventYById[storyEvent.Id] = y;
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

        if (portrayalLinks.Count > 0)
        {
            var titleY = 13 + timelines.Count * 15;
            var firstRowY = titleY + 4;
            root.Child("svg-text", "book-portrayal-rail-title", node => node
                .Text("BOOK PORTRAYAL RAIL · WORLD EVENT → MANUSCRIPT SECTION")
                .Property("x", "4").Property("y", titleY.ToString(CultureInfo.InvariantCulture))
                .Property("font-size", "2").Property("font-weight", "700").Property("fill", "#b7c9df"));

            for (var index = 0; index < portrayalLinks.Count; index++)
            {
                var link = portrayalLinks[index];
                var rowY = firstRowY + index * 8;
                var storyEvent = workspace.Events.First(item => item.Id == link.EventId);
                var eventX = eventXById.GetValueOrDefault(link.EventId, 4);
                if (eventYById.TryGetValue(link.EventId, out var eventY))
                {
                    root.Child("svg-rect", $"portrayal-vertical-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                        .Property("x", (eventX + 7.7).ToString(CultureInfo.InvariantCulture))
                        .Property("y", (eventY + 5).ToString(CultureInfo.InvariantCulture))
                        .Property("width", "0.2")
                        .Property("height", Math.Max(0.2, rowY + 2.5 - (eventY + 5)).ToString(CultureInfo.InvariantCulture))
                        .Property("fill", "#7c9bba"));
                }
                root.Child("svg-rect", $"portrayal-event-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                    .Property("x", eventX.ToString(CultureInfo.InvariantCulture))
                    .Property("y", rowY.ToString(CultureInfo.InvariantCulture))
                    .Property("width", "15.5").Property("height", "5").Property("rx", "0.6")
                    .Property("fill", "#202f43").Property("stroke", "#8bb7e8").Property("stroke-width", "0.25"));
                root.Child("svg-text", $"portrayal-event-label-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                    .Text(Shorten(storyEvent.Title, 12))
                    .Property("x", (eventX + 0.6).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (rowY + 3.2).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.25").Property("fill", "#cfe4ff"));
                root.Child("svg-rect", $"portrayal-connector-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                    .Property("x", (eventX + 15.5).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (rowY + 2.4).ToString(CultureInfo.InvariantCulture))
                    .Property("width", "2").Property("height", "0.2").Property("fill", "#7c9bba"));
                var sectionX = eventX + 17.5;
                root.Child("svg-rect", $"portrayal-section-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                    .Property("x", sectionX.ToString(CultureInfo.InvariantCulture))
                    .Property("y", rowY.ToString(CultureInfo.InvariantCulture))
                    .Property("width", "44").Property("height", "5").Property("rx", "0.6")
                    .Property("fill", "#263329").Property("stroke", "#a9d8c8").Property("stroke-width", "0.25"));
                root.Child("svg-text", $"portrayal-section-label-{link.EventId:N}-{link.ManuscriptId:N}-{link.Section.Id:N}", node => node
                    .Text(Shorten($"{link.ManuscriptTitle}: {link.Section.Title}", 40))
                    .Property("x", (sectionX + 0.6).ToString(CultureInfo.InvariantCulture))
                    .Property("y", (rowY + 3.2).ToString(CultureInfo.InvariantCulture))
                    .Property("font-size", "1.25").Property("fill", "#d7f0e4"));
            }
        }

        return renderer.Render(root.Build(), 1000, (int)(height * 10));
    }

    private static string Shorten(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..Math.Max(1, maxLength - 1)] + "…";
}
