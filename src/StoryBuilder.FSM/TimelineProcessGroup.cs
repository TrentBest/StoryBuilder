using StoryBuilder.Core.Models;
using StoryBuilder.Core.Runtime;
using TheSingularityWorkshop.FSM_API;

namespace StoryBuilder.FSM;

/// <summary>
/// Optional FSM_API-backed executable projection of a canonical timeline definition.
/// Each instance owns an isolated FSM processing group and exposes inspectable event evidence.
/// </summary>
public sealed class TimelineProcessGroup : IStoryProcessGroup, IDisposable
{
    private readonly object _eventGate = new();
    private readonly TimelineProcessGroupContext _context;
    private readonly FSMHandle _handle;
    private readonly string _processingGroup;
    private long _updateCount;
    private bool _disposed;

    public TimelineProcessGroup(TimelineDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (definition.Id == Guid.Empty)
            throw new ArgumentException("Timeline identity must not be empty.", nameof(definition));

        Definition = definition;
        Id = definition.Id;
        Name = string.IsNullOrWhiteSpace(definition.Name) ? "Untitled timeline" : definition.Name.Trim();
        var suffix = Id.ToString("N");
        var definitionName = $"StoryBuilder.Timeline.{suffix}";
        _processingGroup = definitionName;
        _context = new TimelineProcessGroupContext(Id, Name, Record);

        var builder = FSM_API.Create.CreateFiniteStateMachine(definitionName, -1, _processingGroup)
            .State("Running", OnEnter, OnUpdate, OnExit)
            .WithInitialState("Running");
        builder.BuildDefinition();
        _handle = FSM_API.Create.CreateInstance(definitionName, _context, _processingGroup);
    }

    public TimelineDefinition Definition { get; }
    public Guid Id { get; }
    public string Name { get; }
    public long UpdateCount => Interlocked.Read(ref _updateCount);
    public long RuntimeTickCount => Interlocked.Read(ref _context.RuntimeTickCount);

    /// <summary>A snapshot of runtime observations emitted by this timeline only.</summary>
    public IReadOnlyList<TimelineEventRecord> Events
    {
        get
        {
            lock (_eventGate)
                return _context.Events.ToArray();
        }
    }

    /// <summary>Requests one update from FSM_API for this timeline's isolated processing group.</summary>
    public void Update()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Interlocked.Increment(ref _updateCount);
        Record("DriverUpdateRequested", $"Driver requested update {UpdateCount}.");
        FSM_API.Interaction.Update(_processingGroup);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _context.IsValid = false;
        FSM_API.Interaction.DestroyInstance(_handle);
    }

    private void Record(string kind, string description)
    {
        lock (_eventGate)
        {
            _context.Events.Add(new TimelineEventRecord
            {
                TimelineId = Id,
                Sequence = _context.Events.Count + 1L,
                Kind = kind,
                Description = description
            });
        }
    }

    private static void OnEnter(IStateContext context)
    {
        var timeline = (TimelineProcessGroupContext)context;
        timeline.Emit("ProcessGroupEntered", "Timeline Process Group entered its running state.");
    }

    private static void OnUpdate(IStateContext context)
    {
        var timeline = (TimelineProcessGroupContext)context;
        var tick = Interlocked.Increment(ref timeline.RuntimeTickCount);
        timeline.Emit("RuntimeTick", $"Timeline runtime tick {tick}.");
    }

    private static void OnExit(IStateContext context) =>
        ((TimelineProcessGroupContext)context).Emit("ProcessGroupExited", "Timeline Process Group exited its running state.");

    private sealed class TimelineProcessGroupContext(
        Guid timelineId,
        string name,
        Action<string, string> emit) : IStateContext
    {
        public string Name { get; set; } = name;
        public bool IsValid { get; set; } = true;
        public Guid TimelineId { get; } = timelineId;
        public List<TimelineEventRecord> Events { get; } = [];
        public long RuntimeTickCount;
        public void Emit(string kind, string description) => emit(kind, description);
    }
}
