namespace StoryBuilder.Core.Models;

/// <summary>Author-selected narrative perspective for a provider-neutral draft request.</summary>
public enum NarrationPerspective
{
    FirstPerson,
    SecondPerson,
    ThirdPersonLimited,
    ThirdPersonOmniscient
}

/// <summary>
/// A provider-independent snapshot of the material and constraints available to a drafting capability.
/// Construct requests from selected scene beats and an observer-safe knowledge projection; never pass the
/// complete workspace as hidden world context.
/// </summary>
public sealed record NarrationDraftRequest
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public Guid SceneId { get; init; }
    public string SceneTitle { get; init; } = string.Empty;
    public string? Location { get; init; }
    public Guid ManuscriptId { get; init; }
    public string ManuscriptTitle { get; init; } = string.Empty;
    public Guid SectionId { get; init; }
    public string SectionTitle { get; init; } = string.Empty;
    public string ExistingSectionText { get; init; } = string.Empty;
    public NarrationPerspective Perspective { get; init; } = NarrationPerspective.ThirdPersonLimited;
    public string? Voice { get; init; }
    public string? Style { get; init; }
    public Guid? ObserverId { get; init; }
    public Guid? AsOfEventId { get; init; }
    public ObserverKnowledgeProjection? ObserverKnowledge { get; init; }
    public bool AllowOmniscientPerspective { get; init; }
    public IReadOnlyList<NarrationSourceBeat> SourceBeats { get; init; } = Array.Empty<NarrationSourceBeat>();
}

/// <summary>A value snapshot of one selected beat, including its stable source and event identities.</summary>
public sealed record NarrationSourceBeat(
    Guid BeatId,
    int Sequence,
    SceneBeatKind Kind,
    Guid? ActorId,
    string? ActorLabel,
    string? Action,
    string? Reaction,
    string? Dialogue,
    string? PoseOrPosition,
    string? EmotionalTone,
    string? CaptureNote,
    string Attribution,
    IReadOnlyList<Guid> EventIds);

/// <summary>
/// A provider's proposed text. It is not canon and must not be saved until the author accepts it.
/// Provenance segments identify which supplied beats support each range of proposed text.
/// </summary>
public sealed record NarrationDraftProposal
{
    public Guid RequestId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string? ProviderId { get; init; }
    public string? ModelId { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<NarrationDraftSegment> Segments { get; init; } = Array.Empty<NarrationDraftSegment>();
    public IReadOnlyList<NarrationEventClaim> Claims { get; init; } = Array.Empty<NarrationEventClaim>();
}

/// <summary>A character range in proposed text linked to the source beats and events used to draft it.</summary>
public sealed record NarrationDraftSegment(
    int Start,
    int Length,
    IReadOnlyList<Guid> SourceBeatIds,
    IReadOnlyList<Guid> EventIds);

/// <summary>An explicit event assertion made by the proposed narration.</summary>
public sealed record NarrationEventClaim(Guid EventId, NarrationClaimMode Mode);

/// <summary>Validates that provider output refers only to the request and source material it received.</summary>
public static class NarrationDraftProposalValidator
{
    public static void Validate(NarrationDraftRequest request, NarrationDraftProposal proposal)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(proposal);
        if (request.RequestId == Guid.Empty || proposal.RequestId != request.RequestId)
            throw new ArgumentException("The proposal must identify the exact request that produced it.", nameof(proposal));
        if (string.IsNullOrWhiteSpace(proposal.Text))
            throw new ArgumentException("A narration proposal must contain text.", nameof(proposal));
        ArgumentNullException.ThrowIfNull(request.SourceBeats);
        ArgumentNullException.ThrowIfNull(proposal.Segments);
        ArgumentNullException.ThrowIfNull(proposal.Claims);

        if (request.SourceBeats.Any(beat => beat is null || beat.EventIds is null))
            throw new ArgumentException("Every source beat must include its event identity collection.", nameof(request));
        var beatIds = request.SourceBeats.Select(beat => beat.BeatId).ToHashSet();
        if (beatIds.Contains(Guid.Empty) || beatIds.Count != request.SourceBeats.Count)
            throw new ArgumentException("The request contains an empty or duplicate source beat identity.", nameof(request));

        var beatsById = request.SourceBeats.ToDictionary(beat => beat.BeatId);
        var eventIds = request.SourceBeats.SelectMany(beat => beat.EventIds).ToHashSet();
        foreach (var segment in proposal.Segments)
        {
            if (segment is null || segment.SourceBeatIds is null || segment.EventIds is null)
                throw new ArgumentException("Every provenance segment must include source beat and event collections.", nameof(proposal));
            if (segment.Start < 0 || segment.Length <= 0 ||
                segment.Start > proposal.Text.Length - segment.Length)
                throw new ArgumentException("A provenance segment falls outside the proposed text.", nameof(proposal));
            if (segment.SourceBeatIds.Count == 0 ||
                segment.SourceBeatIds.Any(id => !beatIds.Contains(id)))
                throw new ArgumentException("A provenance segment refers to a beat that was not supplied.", nameof(proposal));
            var segmentEventIds = segment.SourceBeatIds
                .SelectMany(id => beatsById[id].EventIds)
                .ToHashSet();
            if (segment.EventIds.Any(id => !segmentEventIds.Contains(id)))
                throw new ArgumentException("A provenance segment links an event not associated with its cited beats.", nameof(proposal));
        }

        foreach (var claim in proposal.Claims)
        {
            if (!eventIds.Contains(claim.EventId))
                throw new ArgumentException("A proposed claim refers to an event that was not supplied.", nameof(proposal));
            if (request.ObserverKnowledge is not null &&
                !request.ObserverKnowledge.Events.Any(item =>
                    item.EventId == claim.EventId &&
                    item.PermittedClaims.Any(permitted => permitted.Mode == claim.Mode)))
                throw new ArgumentException("A proposed claim exceeds the observer knowledge supplied with the request.", nameof(proposal));
        }

        if (request.ObserverId is Guid observerId)
        {
            if (request.ObserverKnowledge is null || request.ObserverKnowledge.ObserverId != observerId)
                throw new ArgumentException("Observer-constrained requests must carry that observer's knowledge projection.", nameof(request));
            if (request.ObserverKnowledge.AsOfEventId != request.AsOfEventId)
                throw new ArgumentException("The observer projection and request must use the same event horizon.", nameof(request));
        }
        else if (request.ObserverKnowledge is not null)
        {
            throw new ArgumentException("An observer knowledge projection cannot be supplied without its observer identity.", nameof(request));
        }

        if (request.Perspective == NarrationPerspective.ThirdPersonOmniscient && !request.AllowOmniscientPerspective)
            throw new ArgumentException("Omniscient perspective requires explicit author permission.", nameof(request));
    }
}
