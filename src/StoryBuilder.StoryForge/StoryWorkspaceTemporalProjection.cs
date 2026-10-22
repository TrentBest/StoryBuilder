using CanonicalStoryEvent = StoryBuilder.Core.Models.StoryEvent;
using StoryBuilder.Core.Models;

namespace StoryBuilder.StoryForge;

/// <summary>
/// Projects canonical StoryBuilder workspace events into Story Forge's query index.
/// StoryBuilder remains the source of truth; the projection does not create or persist duplicate records.
/// </summary>
public static class StoryWorkspaceTemporalProjection
{
    /// <summary>
    /// Builds a temporal query engine from workspace events that have an assigned, existing timeline
    /// and a resolvable numeric coordinate. Textual date labels are deliberately not parsed implicitly.
    /// </summary>
    /// <param name="workspace">The canonical StoryBuilder workspace.</param>
    /// <param name="coordinateResolver">
    /// Resolves an event's authored date/period into a numeric coordinate on its assigned timeline.
    /// Return null when the date is unknown, approximate, or not yet machine-resolvable.
    /// </param>
    public static TemporalQueryEngine CreateEngine(
        StoryWorkspace workspace,
        Func<CanonicalStoryEvent, decimal?> coordinateResolver)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(coordinateResolver);

        var timelines = workspace.Timelines.ToDictionary(timeline => timeline.Id);
        var sources = workspace.Sources.ToDictionary(source => source.Id);
        var projectedEvents = new List<StoryBuilder.StoryForge.StoryEvent>();

        foreach (var item in workspace.Events)
        {
            if (item.TimelineId is not Guid timelineId || !timelines.ContainsKey(timelineId))
                continue;

            var coordinate = coordinateResolver(item);
            if (coordinate is null)
                continue;

            var provenance = item.SourceIds.Select(sourceId =>
            {
                if (sources.TryGetValue(sourceId, out var source))
                {
                    var description = string.IsNullOrWhiteSpace(source.Title)
                        ? "Referenced StoryBuilder source"
                        : source.Title;
                    return new StoryProvenance(sourceId.ToString("D"), description, source.LocationOrUri);
                }

                return new StoryProvenance(sourceId.ToString("D"), "Referenced source record");
            }).ToArray();

            projectedEvents.Add(new StoryBuilder.StoryForge.StoryEvent(
                item.Id.ToString("D"),
                string.IsNullOrWhiteSpace(item.Title) ? "Untitled event" : item.Title,
                new StoryTime(timelineId.ToString("D"), coordinate.Value),
                item.ParticipantIds.Select(id => id.ToString("D")),
                MapCertainty(item.Status.ToString()),
                provenance,
                item.Summary));
        }

        return new TemporalQueryEngine(projectedEvents);
    }

    private static StoryCertainty MapCertainty(string status) => status switch
    {
        "Confirmed" or "Canon" or "Accepted" or "Approved" => StoryCertainty.Confirmed,
        "Inferred" => StoryCertainty.Inferred,
        "Contested" => StoryCertainty.Contested,
        _ => StoryCertainty.Proposed
    };
}
