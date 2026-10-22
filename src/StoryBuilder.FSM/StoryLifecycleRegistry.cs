using StoryBuilder.Core.Models;

namespace StoryBuilder.FSM;

/// <summary>Owns one FSM_API lifecycle session per record identity for a host scope.</summary>
public sealed class StoryLifecycleRegistry : IDisposable
{
    private readonly Dictionary<Guid, StoryLifecycleSession> _sessions = [];
    private bool _disposed;

    public StoryLifecycleSession GetSession(Guid recordId, CanonStatus currentStatus, string? name = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sessions.TryGetValue(recordId, out var existing))
            return existing;

        var session = new StoryLifecycleSession(name);
        foreach (var step in PathFromDraft(currentStatus))
            session.Request(step);
        _sessions.Add(recordId, session);
        return session;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        foreach (var session in _sessions.Values)
            session.Dispose();
        _sessions.Clear();
    }

    private static IEnumerable<CanonStatus> PathFromDraft(CanonStatus status) => status switch
    {
        CanonStatus.Draft => [],
        CanonStatus.Proposed => [CanonStatus.Proposed],
        CanonStatus.Accepted => [CanonStatus.Proposed, CanonStatus.Accepted],
        CanonStatus.Rejected => [CanonStatus.Proposed, CanonStatus.Rejected],
        CanonStatus.Deprecated => [CanonStatus.Proposed, CanonStatus.Accepted, CanonStatus.Deprecated],
        _ => []
    };
}
