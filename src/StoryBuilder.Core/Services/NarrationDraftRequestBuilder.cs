using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Builds a provider-facing narration request from explicitly selected workspace records.
/// Only the selected beat snapshots and, when requested, an observer-safe projection cross this boundary.
/// </summary>
public sealed class NarrationDraftRequestBuilder
{
    private readonly ObserverKnowledgeProjectionService _observerProjectionService;

    public NarrationDraftRequestBuilder(ObserverKnowledgeProjectionService? observerProjectionService = null)
    {
        _observerProjectionService = observerProjectionService ?? new ObserverKnowledgeProjectionService();
    }

    public NarrationDraftRequest Build(
        StoryBuilderSession session,
        Guid sceneId,
        IEnumerable<Guid> selectedBeatIds,
        Guid manuscriptId,
        Guid sectionId,
        NarrationPerspective perspective = NarrationPerspective.ThirdPersonLimited,
        string? voice = null,
        string? style = null,
        Guid? observerId = null,
        Guid? asOfEventId = null,
        bool allowOmniscientPerspective = false)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(selectedBeatIds);
        if (sceneId == Guid.Empty)
            throw new ArgumentException("A scene identity is required.", nameof(sceneId));
        if (manuscriptId == Guid.Empty)
            throw new ArgumentException("A manuscript identity is required.", nameof(manuscriptId));
        if (sectionId == Guid.Empty)
            throw new ArgumentException("A manuscript section identity is required.", nameof(sectionId));
        if (observerId == Guid.Empty)
            throw new ArgumentException("An observer identity cannot be empty.", nameof(observerId));
        if (asOfEventId == Guid.Empty)
            throw new ArgumentException("An event horizon cannot be empty.", nameof(asOfEventId));
        if (asOfEventId is not null && observerId is null)
            throw new ArgumentException("An event horizon requires an observer identity.", nameof(asOfEventId));
        if (perspective == NarrationPerspective.ThirdPersonOmniscient && !allowOmniscientPerspective)
            throw new ArgumentException("Omniscient perspective requires explicit author permission.", nameof(allowOmniscientPerspective));

        var requestedBeatIds = selectedBeatIds.ToArray();
        if (requestedBeatIds.Length == 0)
            throw new ArgumentException("Select at least one scene beat for narration.", nameof(selectedBeatIds));
        if (requestedBeatIds.Any(id => id == Guid.Empty) ||
            requestedBeatIds.Distinct().Count() != requestedBeatIds.Length)
            throw new ArgumentException("Selected beat identities must be non-empty and unique.", nameof(selectedBeatIds));

        var workspace = session.Workspace;
        var scene = workspace.Scenes.FirstOrDefault(item => item.Id == sceneId)
            ?? throw new KeyNotFoundException($"Scene '{sceneId}' does not exist in this workspace.");
        var sceneBeats = scene.Beats
            ?? throw new InvalidOperationException($"Scene '{sceneId}' has no beat collection.");
        var selectedIds = requestedBeatIds.ToHashSet();
        var matchingBeats = sceneBeats.Where(beat => selectedIds.Contains(beat.Id)).ToArray();
        if (matchingBeats.Length != selectedIds.Count)
            throw new KeyNotFoundException("One or more selected scene beats do not exist in the requested scene.");

        var manuscript = workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"Manuscript '{manuscriptId}' does not exist in this workspace.");
        var section = manuscript.Sections.FirstOrDefault(item => item.Id == sectionId)
            ?? throw new KeyNotFoundException($"Section '{sectionId}' does not exist in manuscript '{manuscriptId}'.");

        var sourceBeats = matchingBeats
            .OrderBy(beat => beat.Sequence)
            .Select(beat =>
            {
                if (beat.EventIds is null || beat.EventIds.Any(id => id == Guid.Empty))
                    throw new InvalidOperationException($"Scene beat '{beat.Id}' contains invalid event provenance.");
                return new NarrationSourceBeat(
                    beat.Id,
                    beat.Sequence,
                    beat.Kind,
                    beat.ActorId,
                    beat.ActorLabel,
                    beat.Action,
                    beat.Reaction,
                    beat.Dialogue,
                    beat.PoseOrPosition,
                    beat.EmotionalTone,
                    beat.CaptureNote,
                    beat.Attribution,
                    Array.AsReadOnly(beat.EventIds.Distinct().ToArray()));
            })
            .ToArray();

        var referencedEventIds = sourceBeats.SelectMany(beat => beat.EventIds).Distinct().ToArray();
        var missingEventId = referencedEventIds.FirstOrDefault(id =>
            !workspace.Events.Any(storyEvent => storyEvent.Id == id));
        if (missingEventId != Guid.Empty)
            throw new KeyNotFoundException($"Scene beat provenance references missing event '{missingEventId}'.");

        ObserverKnowledgeProjection? projection = null;
        if (observerId is Guid constrainedObserverId)
        {
            projection = _observerProjectionService.Project(workspace, constrainedObserverId, asOfEventId);
            var knownEventIds = projection.Events
                .Where(item => item.PermittedClaims.Count > 0)
                .Select(item => item.EventId)
                .ToHashSet();

            foreach (var beat in sourceBeats)
            {
                // An unlinked beat or a beat linked to any event outside the observer projection
                // is not safe to expose to an observer-constrained drafting provider.
                if (beat.EventIds.Count == 0 || beat.EventIds.Any(id => !knownEventIds.Contains(id)))
                    throw new InvalidOperationException(
                        $"Scene beat '{beat.BeatId}' is not wholly supported by the selected observer's knowledge projection.");
            }
        }

        return new NarrationDraftRequest
        {
            SceneId = scene.Id,
            SceneTitle = scene.Title,
            Location = scene.Location,
            ManuscriptId = manuscript.Id,
            ManuscriptTitle = manuscript.Title,
            SectionId = section.Id,
            SectionTitle = section.Title,
            ExistingSectionText = section.Body,
            Perspective = perspective,
            Voice = NormalizeOptional(voice),
            Style = NormalizeOptional(style),
            ObserverId = observerId,
            AsOfEventId = asOfEventId,
            ObserverKnowledge = projection,
            AllowOmniscientPerspective = allowOmniscientPerspective,
            SourceBeats = Array.AsReadOnly(sourceBeats)
        };
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
