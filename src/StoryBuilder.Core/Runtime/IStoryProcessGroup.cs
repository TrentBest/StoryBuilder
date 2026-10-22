namespace StoryBuilder.Core.Runtime;

/// <summary>
/// Minimal host-neutral contract for an executable StoryBuilder process group.
/// Concrete runtimes adapt this contract to their execution engine.
/// </summary>
public interface IStoryProcessGroup
{
    Guid Id { get; }
    string Name { get; }
    long UpdateCount { get; }
    void Update();
}
