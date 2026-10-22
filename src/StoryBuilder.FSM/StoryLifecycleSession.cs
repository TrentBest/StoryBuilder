using StoryBuilder.Core.Models;
using TheSingularityWorkshop.FSM_API;

namespace StoryBuilder.FSM;

/// <summary>State context for the review lifecycle of a story record.</summary>
public sealed class StoryLifecycleContext : IStateContext
{
    public string Name { get; set; } = "Story record review";
    public bool IsValid { get; set; } = true;
    public CanonStatus CurrentStatus { get; set; } = CanonStatus.Draft;
    public CanonStatus RequestedStatus { get; set; } = CanonStatus.Draft;
}

/// <summary>
/// Optional FSM_API-backed lifecycle for draft/proposal/acceptance decisions.
/// The domain core remains independent of FSM_API; hosts opt into this adapter.
/// </summary>
public sealed class StoryLifecycleSession : IDisposable
{
    private const string DefinitionName = "StoryBuilder.RecordLifecycle";
    private const string ProcessingGroup = "StoryBuilder.RecordLifecycle";
    private static readonly object DefinitionGate = new();
    private static bool _definitionCreated;
    private readonly StoryLifecycleContext _context;
    private readonly FSMHandle _handle;

    public StoryLifecycleSession(string? name = null)
    {
        EnsureDefinition();
        _context = new StoryLifecycleContext
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Story record review" : name.Trim()
        };
        _handle = FSM_API.Create.CreateInstance(DefinitionName, _context, ProcessingGroup);
        // FSM_API enters the initial state on the first update; initialize it before accepting requests.
        FSM_API.Interaction.Update(ProcessingGroup);
    }

    public CanonStatus CurrentStatus => _context.CurrentStatus;

    /// <summary>Requests a status and advances the FSM once; disallowed transitions leave the current status unchanged.</summary>
    public CanonStatus Request(CanonStatus requestedStatus)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _context.RequestedStatus = requestedStatus;
        FSM_API.Interaction.Update(ProcessingGroup);
        // FSM_API schedules the destination state's OnEnter for the next group update after a transition.
        if (!string.Equals(_handle.CurrentState, _context.CurrentStatus.ToString(), StringComparison.Ordinal))
            FSM_API.Interaction.Update(ProcessingGroup);
        return _context.CurrentStatus;
    }

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _context.IsValid = false;
        FSM_API.Interaction.DestroyInstance(_handle);
    }

    private static void EnsureDefinition()
    {
        if (_definitionCreated)
            return;

        lock (DefinitionGate)
        {
            if (_definitionCreated)
                return;

            var builder = FSM_API.Create.CreateFiniteStateMachine(DefinitionName, 0, ProcessingGroup)
                .State(nameof(CanonStatus.Draft), ctx => Enter(ctx, CanonStatus.Draft), _ => { }, _ => { })
                .State(nameof(CanonStatus.Proposed), ctx => Enter(ctx, CanonStatus.Proposed), _ => { }, _ => { })
                .State(nameof(CanonStatus.Accepted), ctx => Enter(ctx, CanonStatus.Accepted), _ => { }, _ => { })
                .State(nameof(CanonStatus.Rejected), ctx => Enter(ctx, CanonStatus.Rejected), _ => { }, _ => { })
                .State(nameof(CanonStatus.Deprecated), ctx => Enter(ctx, CanonStatus.Deprecated), _ => { }, _ => { })
                .WithInitialState(nameof(CanonStatus.Draft));

            builder
                .Transition(nameof(CanonStatus.Draft), nameof(CanonStatus.Proposed), ctx => Requested(ctx, CanonStatus.Proposed))
                .Transition(nameof(CanonStatus.Proposed), nameof(CanonStatus.Draft), ctx => Requested(ctx, CanonStatus.Draft))
                .Transition(nameof(CanonStatus.Proposed), nameof(CanonStatus.Accepted), ctx => Requested(ctx, CanonStatus.Accepted))
                .Transition(nameof(CanonStatus.Proposed), nameof(CanonStatus.Rejected), ctx => Requested(ctx, CanonStatus.Rejected))
                .Transition(nameof(CanonStatus.Rejected), nameof(CanonStatus.Draft), ctx => Requested(ctx, CanonStatus.Draft))
                .Transition(nameof(CanonStatus.Rejected), nameof(CanonStatus.Proposed), ctx => Requested(ctx, CanonStatus.Proposed))
                .Transition(nameof(CanonStatus.Accepted), nameof(CanonStatus.Proposed), ctx => Requested(ctx, CanonStatus.Proposed))
                .Transition(nameof(CanonStatus.Accepted), nameof(CanonStatus.Deprecated), ctx => Requested(ctx, CanonStatus.Deprecated));

            builder.BuildDefinition();
            _definitionCreated = true;
        }
    }

    private static void Enter(IStateContext context, CanonStatus status) =>
        ((StoryLifecycleContext)context).CurrentStatus = status;

    private static bool Requested(IStateContext context, CanonStatus status) =>
        ((StoryLifecycleContext)context).RequestedStatus == status;
}
