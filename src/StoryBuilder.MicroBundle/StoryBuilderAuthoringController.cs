using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;

namespace StoryBuilder.MicroBundle;

/// <summary>
/// Defines the small, explicit command vocabulary that desktop and other hosts may bind to controls.
/// </summary>
/// <remarks>
/// Command identifiers are stable integration contracts. This controller deliberately does not
/// interpret arbitrary method names or invoke members through reflection.
/// </remarks>
public static class StoryBuilderAuthoringCommands
{
    public const string CreateTimeline = "storybuilder.timeline.create";
    public const string CreateCharacter = "storybuilder.character.create";
    public const string CreateScene = "storybuilder.scene.create";
    public const string CreateManuscript = "storybuilder.manuscript.create";
    public const string CreateEvent = "storybuilder.event.create";
    public const string CreateManuscriptSection = "storybuilder.manuscript.section.create";
    public const string LinkLatestEventToLatestSection = "storybuilder.manuscript.section.link-latest-event";
}

/// <summary>Reports the outcome of an explicit StoryBuilder authoring command.</summary>
public sealed record StoryBuilderCommandResult(
    bool Succeeded,
    string CommandId,
    string Message,
    Guid? RecordId = null);

/// <summary>Applies a bounded set of host-originated commands to one isolated authoring session.</summary>
public sealed class StoryBuilderAuthoringController
{
    public StoryBuilderAuthoringController(StoryBuilderSession session)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <summary>The session owned by this controller.</summary>
    public StoryBuilderSession Session { get; }

    /// <summary>
    /// Executes a supported command. The optional value supplies the new record's display name.
    /// Unknown commands are rejected without mutating the workspace.
    /// </summary>
    public StoryBuilderCommandResult Execute(string commandId, string? value = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandId);
        var name = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        return commandId switch
        {
            StoryBuilderAuthoringCommands.CreateTimeline => CreateTimeline(name),
            StoryBuilderAuthoringCommands.CreateCharacter => CreateCharacter(name),
            StoryBuilderAuthoringCommands.CreateScene => CreateScene(name),
            StoryBuilderAuthoringCommands.CreateManuscript => CreateManuscript(name),
            StoryBuilderAuthoringCommands.CreateEvent => CreateEvent(name),
            StoryBuilderAuthoringCommands.CreateManuscriptSection => CreateManuscriptSection(name),
            StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection => LinkLatestEventToLatestSection(),
            _ => new StoryBuilderCommandResult(
                false, commandId,
                $"Unsupported StoryBuilder command '{commandId}'. No changes were made.")
        };
    }

    private StoryBuilderCommandResult CreateTimeline(string? name)
    {
        var timeline = Session.CreateTimeline(name ?? $"Timeline {Session.Workspace.Timelines.Count + 1}");
        return new(true, StoryBuilderAuthoringCommands.CreateTimeline,
            $"Created timeline '{timeline.Name}'.", timeline.Id);
    }

    private StoryBuilderCommandResult CreateCharacter(string? name)
    {
        var character = Session.CreateCharacter(name ?? $"Character {Session.Workspace.Characters.Count + 1}");
        return new(true, StoryBuilderAuthoringCommands.CreateCharacter,
            $"Created character '{character.Name}'.", character.Id);
    }

    private StoryBuilderCommandResult CreateScene(string? name)
    {
        var scene = Session.CreateScene(name ?? $"Scene {Session.Workspace.Scenes.Count + 1}");
        return new(true, StoryBuilderAuthoringCommands.CreateScene,
            $"Created scene '{scene.Title}'.", scene.Id);
    }

    private StoryBuilderCommandResult CreateManuscript(string? name)
    {
        var manuscript = Session.CreateManuscript(name ?? $"Book {Session.Workspace.Manuscripts.Count + 1}");
        return new(true, StoryBuilderAuthoringCommands.CreateManuscript,
            $"Created manuscript '{manuscript.Title}'.", manuscript.Id);
    }

    private StoryBuilderCommandResult CreateEvent(string? name)
    {
        var timeline = Session.Workspace.Timelines.FirstOrDefault()
            ?? Session.CreateTimeline("Main Timeline");
        var storyEvent = Session.CreateTimelineEvent(
            timeline.Id,
            name ?? $"Event {Session.Workspace.Events.Count + 1}");
        return new(true, StoryBuilderAuthoringCommands.CreateEvent,
            $"Created event '{storyEvent.Title}' on timeline '{timeline.Name}'.", storyEvent.Id);
    }

    private StoryBuilderCommandResult CreateManuscriptSection(string? name)
    {
        var manuscript = Session.Workspace.Manuscripts.FirstOrDefault()
            ?? Session.CreateManuscript("Untitled Book");
        var updated = Session.AddManuscriptSection(
            manuscript.Id,
            ManuscriptSectionKind.Passage,
            name ?? $"Passage {manuscript.Sections.Count + 1}");
        var section = updated.Sections.OrderByDescending(item => item.Sequence).First();
        return new(true, StoryBuilderAuthoringCommands.CreateManuscriptSection,
            $"Created passage '{section.Title}' in '{updated.Title}'.", section.Id);
    }

    private StoryBuilderCommandResult LinkLatestEventToLatestSection()
    {
        var storyEvent = Session.Workspace.Events.LastOrDefault();
        var manuscript = Session.Workspace.Manuscripts.LastOrDefault();
        var section = manuscript?.Sections.OrderBy(item => item.Sequence).LastOrDefault();

        if (storyEvent is null || manuscript is null || section is null)
        {
            return new(false, StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection,
                "Create an event and a manuscript passage before linking them. No changes were made.");
        }

        Session.LinkEventToManuscriptSection(manuscript.Id, section.Id, storyEvent.Id);
        return new(true, StoryBuilderAuthoringCommands.LinkLatestEventToLatestSection,
            $"Linked event '{storyEvent.Title}' to passage '{section.Title}'.", section.Id);
    }
}
