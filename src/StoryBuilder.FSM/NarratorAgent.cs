using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using TheSingularityWorkshop.FSM_API;

namespace StoryBuilder.FSM;

/// <summary>A user-visible event emitted by one of the Narrator agent's cooperating FSMs.</summary>
public sealed record NarratorAgentEvent(long Sequence, string Machine, string Kind, string Detail, DateTimeOffset AtUtc);

/// <summary>Result of submitting a draft with its explicitly declared world-event claims.</summary>
public sealed record NarratorDraftSubmission(
    bool Accepted,
    IReadOnlyList<ObserverKnowledgeEvaluation> Constraints,
    string Message);

/// <summary>
/// Coordinates focused FSM_API state machines for intent, planning, observer constraints, and draft review.
/// Domain records remain authoritative; the agent orchestrates transitions and records evidence.
/// </summary>
public sealed class NarratorAgent : IDisposable
{
    private const string IntentDefinitionPrefix = "StoryBuilder.Narrator.Intent";
    private const string PlanningDefinitionPrefix = "StoryBuilder.Narrator.Planning";
    private const string ObserverDefinitionPrefix = "StoryBuilder.Narrator.Observer";
    private const string DraftDefinitionPrefix = "StoryBuilder.Narrator.DraftReview";

    private readonly StoryBuilderSession _session;
    private readonly ObserverKnowledgeEvaluator _knowledgeEvaluator = new();
    private readonly object _eventGate = new();
    private readonly List<NarratorAgentEvent> _events = [];
    private readonly string _intentDefinition;
    private readonly string _planningDefinition;
    private readonly string _observerDefinition;
    private readonly string _draftDefinition;
    private readonly string _intentGroup;
    private readonly string _planningGroup;
    private readonly string _observerGroup;
    private readonly string _draftGroup;
    private readonly NarratorFsmContext _intent;
    private readonly NarratorFsmContext _planning;
    private readonly NarratorFsmContext _observer;
    private readonly NarratorFsmContext _draft;
    private readonly FSMHandle _intentHandle;
    private readonly FSMHandle _planningHandle;
    private readonly FSMHandle _observerHandle;
    private readonly FSMHandle _draftHandle;
    private Guid? _draftManuscriptId;
    private Guid? _draftSectionId;
    private bool _allowOmniscientDraft;
    private bool _disposed;

    public NarratorAgent(StoryBuilderSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        var suffix = Guid.NewGuid().ToString("N");
        _intentDefinition = $"{IntentDefinitionPrefix}.{suffix}";
        _planningDefinition = $"{PlanningDefinitionPrefix}.{suffix}";
        _observerDefinition = $"{ObserverDefinitionPrefix}.{suffix}";
        _draftDefinition = $"{DraftDefinitionPrefix}.{suffix}";
        _intentGroup = _intentDefinition;
        _planningGroup = _planningDefinition;
        _observerGroup = _observerDefinition;
        _draftGroup = _draftDefinition;
        BuildIntentDefinition(_intentDefinition, _intentGroup);
        BuildPlanningDefinition(_planningDefinition, _planningGroup);
        BuildObserverDefinition(_observerDefinition, _observerGroup);
        BuildDraftDefinition(_draftDefinition, _draftGroup);
        _intent = new NarratorFsmContext("Intent", Emit);
        _planning = new NarratorFsmContext("Planning", Emit);
        _observer = new NarratorFsmContext("Observer", Emit);
        _draft = new NarratorFsmContext("DraftReview", Emit);
        _intentHandle = FSM_API.Create.CreateInstance(_intentDefinition, _intent, _intentGroup);
        _planningHandle = FSM_API.Create.CreateInstance(_planningDefinition, _planning, _planningGroup);
        _observerHandle = FSM_API.Create.CreateInstance(_observerDefinition, _observer, _observerGroup);
        _draftHandle = FSM_API.Create.CreateInstance(_draftDefinition, _draft, _draftGroup);
        Initialize(_intentGroup);
        Initialize(_planningGroup);
        Initialize(_observerGroup);
        Initialize(_draftGroup);
    }

    public string IntentState => _intent.CurrentState;
    public string PlanningState => _planning.CurrentState;
    public string ObserverState => _observer.CurrentState;
    public string DraftState => _draft.CurrentState;
    public Guid? CurrentPremiseId { get; private set; }
    public Guid? LastPlanningPointId { get; private set; }
    public Guid? SelectedObserverId => _observer.ObserverId;
    public ObserverKnowledgeEvaluation? LastObserverEvaluation { get; private set; }

    public IReadOnlyList<NarratorAgentEvent> Events
    {
        get
        {
            lock (_eventGate)
                return _events.ToArray();
        }
    }

    /// <summary>Captures the author's idea as a premise proposal and advances the intent FSM.</summary>
    public StoryPremise CaptureIntent(
        string intent,
        string? title = null,
        string? scale = null,
        string? genre = null,
        string? creationApproach = null,
        string? primaryTarget = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(intent);
        var premise = _session.CreatePremise(intent, title, scale, genre, creationApproach, primaryTarget);
        CurrentPremiseId = premise.Id;
        _intent.PremiseId = premise.Id;
        _intent.IntentText = premise.Vision;
        if (_intent.CurrentState == "Idle" || _intent.CurrentState == "Ready")
            Move(_intent, _intentHandle, _intentGroup, "Captured");
        Emit("Intent", "PremiseCaptured", $"Captured premise proposal {premise.Id}.");
        return premise;
    }

    /// <summary>Synchronizes the intent FSM to a premise already present in a restored workspace.</summary>
    public void AdoptPremise(StoryPremise premise)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(premise);
        CurrentPremiseId = premise.Id;
        _intent.PremiseId = premise.Id;
        _intent.IntentText = premise.Vision;
        if (_intent.CurrentState == "Idle")
            Move(_intent, _intentHandle, _intentGroup, "Captured");
        var target = premise.Status == CanonStatus.Accepted ? "Ready" : "Captured";
        if (_intent.CurrentState != target)
            Move(_intent, _intentHandle, _intentGroup, target);
        Emit("Intent", "PremiseSelected", $"Selected premise proposal {premise.Id}.");
    }

    public void MarkIntentReady()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Move(_intent, _intentHandle, _intentGroup, "Ready");
    }

    /// <summary>Adds an incomplete planning point without requiring its meaning to be known.</summary>
    public TimelinePlanningPoint AddPlanningPoint(
        Guid timelineId,
        string? label = null,
        string? approximatePlacement = null,
        string? intent = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var point = _session.CreatePlanningPoint(timelineId, label, approximatePlacement, intent);
        LastPlanningPointId = point.Id;
        _planning.PlanningPointId = point.Id;
        if (_planning.CurrentState is "Empty" or "Specified")
            Move(_planning, _planningHandle, _planningGroup, "Anchored");
        Emit("Planning", "PlanningPointAdded", $"Added provisional point {point.Id} without declaring canon.");
        return point;
    }

    public void MarkPlanSpecified()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Move(_planning, _planningHandle, _planningGroup, "Specified");
    }

    public void SelectObserver(Guid observerId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_session.Workspace.Characters.Any(character => character.Id == observerId))
            throw new KeyNotFoundException($"Observer character '{observerId}' does not exist in this workspace.");
        _observer.ObserverId = observerId;
        if (_observer.CurrentState != "Selected")
            Move(_observer, _observerHandle, _observerGroup, "Selected");
        Emit("Observer", "ObserverSelected", $"Selected observer {observerId}.");
    }

    public void ClearObserver()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _observer.ObserverId = null;
        LastObserverEvaluation = null;
        if (_observer.CurrentState != "Unselected")
            Move(_observer, _observerHandle, _observerGroup, "Unselected");
    }

    public ObserverKnowledgeEvaluation EvaluateObserverClaim(Guid eventId, Guid? asOfEventId, NarrationClaimMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_observer.ObserverId is not Guid observerId)
            throw new InvalidOperationException("Select an observer before evaluating an observer-constrained claim.");
        var result = _knowledgeEvaluator.Evaluate(_session.Workspace, observerId, eventId, asOfEventId, mode);
        LastObserverEvaluation = result;
        Move(_observer, _observerHandle, _observerGroup, result.IsAllowed ? "Allowed" : "Blocked");
        Emit("Observer", result.IsAllowed ? "ClaimAllowed" : result.IsUnresolved ? "ClaimUnresolved" : "ClaimBlocked", result.Explanation);
        return result;
    }

    /// <summary>Begins a draft operation for a specific manuscript section.</summary>
    public void RequestDraft(Guid manuscriptId, Guid sectionId, Guid? observerId = null, bool allowOmniscient = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var manuscript = _session.Workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"Manuscript '{manuscriptId}' does not exist.");
        if (!manuscript.Sections.Any(section => section.Id == sectionId))
            throw new KeyNotFoundException($"Section '{sectionId}' does not exist in the selected manuscript.");
        if (observerId is Guid selected)
            SelectObserver(selected);
        else
            ClearObserver();
        _draftManuscriptId = manuscriptId;
        _draftSectionId = sectionId;
        _allowOmniscientDraft = allowOmniscient;
        if (_draft.CurrentState != "Requested")
            Move(_draft, _draftHandle, _draftGroup, "Requested");
        Emit("DraftReview", "DraftRequested", $"Requested draft for manuscript section {sectionId}.");
    }

    /// <summary>
    /// Saves a draft only if its declared event claims pass the selected observer's constraints,
    /// or the caller explicitly selected an omniscient policy.
    /// </summary>
    public NarratorDraftSubmission SubmitDraft(
        string draftText,
        IEnumerable<Guid> assertedEventIds,
        Guid? asOfEventId,
        NarrationClaimMode claimMode = NarrationClaimMode.DirectFact)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(draftText);
        ArgumentNullException.ThrowIfNull(assertedEventIds);
        if (_draft.CurrentState != "Requested" || _draftManuscriptId is not Guid manuscriptId || _draftSectionId is not Guid sectionId)
            throw new InvalidOperationException("Request a manuscript draft before submitting its text.");

        var eventIds = assertedEventIds.Distinct().ToList();
        if (eventIds.Any(eventId => !_session.Workspace.Events.Any(storyEvent => storyEvent.Id == eventId)))
        {
            Move(_draft, _draftHandle, _draftGroup, "NeedsReview");
            return new NarratorDraftSubmission(false, [], "The draft references a world event that does not exist in this workspace.");
        }

        var evaluations = new List<ObserverKnowledgeEvaluation>();
        if (eventIds.Count > 0 && _observer.ObserverId is not Guid && !_allowOmniscientDraft)
        {
            Move(_draft, _draftHandle, _draftGroup, "NeedsReview");
            return new NarratorDraftSubmission(false, [],
                "Select an observer or explicitly choose an omniscient narration policy before asserting world events.");
        }

        if (_observer.ObserverId is Guid observerId)
        {
            evaluations.AddRange(eventIds.Select(eventId =>
                _knowledgeEvaluator.Evaluate(_session.Workspace, observerId, eventId, asOfEventId, claimMode)));
            if (evaluations.Any(result => !result.IsAllowed))
            {
                Move(_draft, _draftHandle, _draftGroup, "NeedsReview");
                return new NarratorDraftSubmission(false, evaluations,
                    "Draft held for review because one or more event claims exceed the selected observer's accepted knowledge.");
            }
        }

        var manuscript = _session.Workspace.Manuscripts.Single(item => item.Id == manuscriptId);
        var section = manuscript.Sections.Single(item => item.Id == sectionId);
        _session.UpdateManuscriptSection(manuscriptId, sectionId, section.Title, draftText);
        foreach (var eventId in eventIds)
            _session.LinkEventToManuscriptSection(manuscriptId, sectionId, eventId);
        _draft.DraftText = draftText;
        Move(_draft, _draftHandle, _draftGroup, "Ready");
        Emit("DraftReview", "DraftSaved", $"Saved draft text and linked {eventIds.Count} declared world-event claim(s).");
        return new NarratorDraftSubmission(true, evaluations, "Draft saved to the manuscript section.");
    }

    public void RequestRevision()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Move(_draft, _draftHandle, _draftGroup, "Requested");
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _intent.IsValid = false;
        _planning.IsValid = false;
        _observer.IsValid = false;
        _draft.IsValid = false;
        FSM_API.Interaction.DestroyInstance(_intentHandle);
        FSM_API.Interaction.DestroyInstance(_planningHandle);
        FSM_API.Interaction.DestroyInstance(_observerHandle);
        FSM_API.Interaction.DestroyInstance(_draftHandle);
        FSM_API.Interaction.DestroyFiniteStateMachine(_intentDefinition, _intentGroup);
        FSM_API.Interaction.DestroyFiniteStateMachine(_planningDefinition, _planningGroup);
        FSM_API.Interaction.DestroyFiniteStateMachine(_observerDefinition, _observerGroup);
        FSM_API.Interaction.DestroyFiniteStateMachine(_draftDefinition, _draftGroup);
    }

    private static void Initialize(string group) => FSM_API.Interaction.Update(group);

    private static void Move(NarratorFsmContext context, FSMHandle handle, string group, string target)
    {
        if (context.CurrentState == target)
            return;
        context.RequestedState = target;
        FSM_API.Interaction.Update(group);
        if (!string.Equals(handle.CurrentState, context.CurrentState, StringComparison.Ordinal))
            FSM_API.Interaction.Update(group);
        if (!string.Equals(context.CurrentState, target, StringComparison.Ordinal))
            throw new InvalidOperationException($"Narrator FSM '{context.Name}' could not transition from '{context.CurrentState}' to '{target}'.");
    }

    private void Emit(string machine, string kind, string detail)
    {
        lock (_eventGate)
            _events.Add(new NarratorAgentEvent(_events.Count + 1L, machine, kind, detail, DateTimeOffset.UtcNow));
    }

    private static void BuildIntentDefinition(string definitionName, string processingGroup)
    {
        var builder = FSM_API.Create.CreateFiniteStateMachine(definitionName, -1, processingGroup)
            .State("Idle", context => Enter(context, "Idle"), _ => { }, _ => { })
            .State("Captured", context => Enter(context, "Captured"), _ => { }, _ => { })
            .State("Ready", context => Enter(context, "Ready"), _ => { }, _ => { })
            .WithInitialState("Idle");
        builder.Transition("Idle", "Captured", context => Requested(context, "Captured"))
            .Transition("Captured", "Ready", context => Requested(context, "Ready"))
            .Transition("Ready", "Captured", context => Requested(context, "Captured"));
        builder.BuildDefinition();
    }

    private static void BuildPlanningDefinition(string definitionName, string processingGroup)
    {
        var builder = FSM_API.Create.CreateFiniteStateMachine(definitionName, -1, processingGroup)
            .State("Empty", context => Enter(context, "Empty"), _ => { }, _ => { })
            .State("Anchored", context => Enter(context, "Anchored"), _ => { }, _ => { })
            .State("Specified", context => Enter(context, "Specified"), _ => { }, _ => { })
            .WithInitialState("Empty");
        builder.Transition("Empty", "Anchored", context => Requested(context, "Anchored"))
            .Transition("Anchored", "Specified", context => Requested(context, "Specified"))
            .Transition("Specified", "Anchored", context => Requested(context, "Anchored"));
        builder.BuildDefinition();
    }

    private static void BuildObserverDefinition(string definitionName, string processingGroup)
    {
        var builder = FSM_API.Create.CreateFiniteStateMachine(definitionName, -1, processingGroup)
            .State("Unselected", context => Enter(context, "Unselected"), _ => { }, _ => { })
            .State("Selected", context => Enter(context, "Selected"), _ => { }, _ => { })
            .State("Allowed", context => Enter(context, "Allowed"), _ => { }, _ => { })
            .State("Blocked", context => Enter(context, "Blocked"), _ => { }, _ => { })
            .WithInitialState("Unselected");
        builder.Transition("Unselected", "Selected", context => Requested(context, "Selected"))
            .Transition("Selected", "Allowed", context => Requested(context, "Allowed"))
            .Transition("Selected", "Blocked", context => Requested(context, "Blocked"))
            .Transition("Selected", "Unselected", context => Requested(context, "Unselected"))
            .Transition("Allowed", "Selected", context => Requested(context, "Selected"))
            .Transition("Allowed", "Blocked", context => Requested(context, "Blocked"))
            .Transition("Allowed", "Unselected", context => Requested(context, "Unselected"))
            .Transition("Blocked", "Selected", context => Requested(context, "Selected"))
            .Transition("Blocked", "Allowed", context => Requested(context, "Allowed"))
            .Transition("Blocked", "Unselected", context => Requested(context, "Unselected"));
        builder.BuildDefinition();
    }

    private static void BuildDraftDefinition(string definitionName, string processingGroup)
    {
        var builder = FSM_API.Create.CreateFiniteStateMachine(definitionName, -1, processingGroup)
            .State("Idle", context => Enter(context, "Idle"), _ => { }, _ => { })
            .State("Requested", context => Enter(context, "Requested"), _ => { }, _ => { })
            .State("Ready", context => Enter(context, "Ready"), _ => { }, _ => { })
            .State("NeedsReview", context => Enter(context, "NeedsReview"), _ => { }, _ => { })
            .WithInitialState("Idle");
        builder.Transition("Idle", "Requested", context => Requested(context, "Requested"))
            .Transition("Requested", "Ready", context => Requested(context, "Ready"))
            .Transition("Requested", "NeedsReview", context => Requested(context, "NeedsReview"))
            .Transition("Ready", "Requested", context => Requested(context, "Requested"))
            .Transition("Ready", "NeedsReview", context => Requested(context, "NeedsReview"))
            .Transition("NeedsReview", "Requested", context => Requested(context, "Requested"))
            .Transition("NeedsReview", "Ready", context => Requested(context, "Ready"));
        builder.BuildDefinition();
    }

    private static void Enter(IStateContext context, string state) => ((NarratorFsmContext)context).Enter(state);
    private static bool Requested(IStateContext context, string state) => ((NarratorFsmContext)context).RequestedState == state;

    private sealed class NarratorFsmContext(string name, Action<string, string, string> emit) : IStateContext
    {
        private readonly Action<string, string, string> _emit = emit;
        public string Name { get; set; } = name;
        public bool IsValid { get; set; } = true;
        public string CurrentState { get; private set; } = string.Empty;
        public string RequestedState { get; set; } = string.Empty;
        public Guid? PremiseId { get; set; }
        public string IntentText { get; set; } = string.Empty;
        public Guid? PlanningPointId { get; set; }
        public Guid? ObserverId { get; set; }
        public string DraftText { get; set; } = string.Empty;

        public void Enter(string state)
        {
            var previous = CurrentState;
            CurrentState = state;
            if (!string.Equals(previous, state, StringComparison.Ordinal))
                _emit(Name, "StateChanged", string.IsNullOrEmpty(previous) ? state : $"{previous} → {state}");
        }
    }
}
