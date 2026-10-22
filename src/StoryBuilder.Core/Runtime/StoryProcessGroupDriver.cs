namespace StoryBuilder.Core.Runtime;

/// <summary>
/// A minimal host-neutral driver that updates the process groups inserted into it.
/// The host owns driver scheduling; this type does not create a timer or background loop.
/// </summary>
public sealed class StoryProcessGroupDriver
{
    private readonly Dictionary<Guid, IStoryProcessGroup> _groups = [];

    public IReadOnlyCollection<IStoryProcessGroup> Groups => _groups.Values.ToArray();

    public void Insert(IStoryProcessGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);
        if (!_groups.TryAdd(group.Id, group))
            throw new InvalidOperationException($"A process group with ID '{group.Id}' is already inserted.");
    }

    public bool Remove(Guid groupId) => _groups.Remove(groupId);

    /// <summary>Advances a stable snapshot so group changes do not invalidate the current update pass.</summary>
    public void Update()
    {
        foreach (var group in _groups.Values.ToArray())
            group.Update();
    }
}
