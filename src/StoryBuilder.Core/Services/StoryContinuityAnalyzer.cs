using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Finds narrative continuity conflicts that can be identified without guessing dates or canon.
/// Results are advisory; analysis never edits or resolves the author's records.
/// </summary>
public static class StoryContinuityAnalyzer
{
    public static IReadOnlyList<StoryContinuityFinding> Analyze(StoryWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var findings = new List<StoryContinuityFinding>();
        var events = workspace.Events.ToDictionarySafe(eventRecord => eventRecord.Id);
        var timelines = workspace.Timelines.ToDictionarySafe(timeline => timeline.Id);
        var places = workspace.Places.ToDictionarySafe(place => place.Id);

        foreach (var storyEvent in workspace.Events)
        {
            if (storyEvent.CauseEventIds.Contains(storyEvent.Id))
                findings.Add(new StoryContinuityFinding("event.self-causation", storyEvent.Id,
                    "An event cannot be its own cause.", [storyEvent.Id]));

            foreach (var causeId in storyEvent.CauseEventIds.Distinct())
            {
                if (causeId == storyEvent.Id || !events.TryGetValue(causeId, out var cause))
                    continue;

                if (!cause.ConsequenceEventIds.Contains(storyEvent.Id))
                    findings.Add(new StoryContinuityFinding("event.cause-link-not-reciprocal", storyEvent.Id,
                        $"Event '{storyEvent.Title}' lists '{cause.Title}' as a cause, but the cause does not list this event as a consequence.",
                        [causeId, storyEvent.Id]));
            }

            foreach (var consequenceId in storyEvent.ConsequenceEventIds.Distinct())
            {
                if (consequenceId == storyEvent.Id || !events.TryGetValue(consequenceId, out var consequence))
                    continue;

                if (!consequence.CauseEventIds.Contains(storyEvent.Id))
                    findings.Add(new StoryContinuityFinding("event.consequence-link-not-reciprocal", storyEvent.Id,
                        $"Event '{storyEvent.Title}' lists '{consequence.Title}' as a consequence, but that event does not list this event as a cause.",
                        [storyEvent.Id, consequenceId]));
            }
        }

        foreach (var cycle in FindCycles(events.Keys, id => events.TryGetValue(id, out var item)
                     ? item.CauseEventIds.Where(events.ContainsKey)
                     : Enumerable.Empty<Guid>()))
        {
            findings.Add(new StoryContinuityFinding("event.causal-cycle", cycle[0],
                "The causal links form a cycle. Review the linked events before treating the sequence as established.",
                cycle));
        }

        foreach (var cycle in FindCycles(timelines.Keys, id => timelines.TryGetValue(id, out var item) &&
                     item.ParentTimelineId is Guid parent && timelines.ContainsKey(parent)
                         ? [parent]
                         : Enumerable.Empty<Guid>()))
        {
            findings.Add(new StoryContinuityFinding("timeline.branch-cycle", cycle[0],
                "The timeline parent links form a cycle, so this branch does not lead back to a root timeline.",
                cycle));
        }

        foreach (var cycle in FindCycles(places.Keys, id => places.TryGetValue(id, out var item) &&
                     item.ParentPlaceId is Guid parent && places.ContainsKey(parent)
                         ? [parent]
                         : Enumerable.Empty<Guid>()))
        {
            findings.Add(new StoryContinuityFinding("place.containment-cycle", cycle[0],
                "The place containment links form a cycle, so the hierarchy has no valid root for this chain.",
                cycle));
        }

        return findings;
    }

    private static IReadOnlyList<IReadOnlyList<Guid>> FindCycles(
        IEnumerable<Guid> nodes, Func<Guid, IEnumerable<Guid>> edges)
    {
        var results = new List<IReadOnlyList<Guid>>();
        var state = new Dictionary<Guid, byte>();
        var stack = new List<Guid>();
        var emitted = new HashSet<string>(StringComparer.Ordinal);

        void Visit(Guid node)
        {
            state[node] = 1;
            stack.Add(node);
            foreach (var next in edges(node))
            {
                if (!state.TryGetValue(next, out var nextState))
                {
                    Visit(next);
                }
                else if (nextState == 1)
                {
                    var start = stack.LastIndexOf(next);
                    var cycle = stack.Skip(start).ToArray();
                    var key = string.Join("|", cycle.OrderBy(id => id).Select(id => id.ToString("N")));
                    if (emitted.Add(key))
                        results.Add(cycle);
                }
            }
            stack.RemoveAt(stack.Count - 1);
            state[node] = 2;
        }

        foreach (var node in nodes)
            if (!state.ContainsKey(node))
                Visit(node);

        return results;
    }
}

internal static class StoryContinuityDictionaryExtensions
{
    public static Dictionary<Guid, T> ToDictionarySafe<T>(this IEnumerable<T> items, Func<T, Guid> key)
    {
        var result = new Dictionary<Guid, T>();
        foreach (var item in items)
            result.TryAdd(key(item), item);
        return result;
    }
}

/// <summary>A reviewable narrative continuity finding. Record IDs identify the affected records.</summary>
public sealed record StoryContinuityFinding(
    string Code,
    Guid PrimaryRecordId,
    string Message,
    IReadOnlyList<Guid> RelatedRecordIds);
