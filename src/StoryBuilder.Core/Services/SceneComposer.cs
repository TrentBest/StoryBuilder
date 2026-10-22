using System.Text;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>Captures scene beats and turns only recorded material into a narration draft.</summary>
public sealed class SceneComposer
{
    public SceneRecord Create(string? title = null, string? location = null, SceneCaptureMode mode = SceneCaptureMode.Freeform) =>
        new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Untitled scene" : title.Trim(),
            Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim(),
            CaptureMode = mode
        };

    public SceneRecord AddBeat(SceneRecord scene, SceneBeat beat)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(beat);
        return scene.AddBeat(beat);
    }

    /// <summary>
    /// Produces an editable first-pass narration from explicit beat fields.
    /// It does not invent missing actions, motivations, outcomes, or sensory details.
    /// </summary>
    public string ComposeNarration(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        var lines = new List<string>();

        foreach (var beat in scene.Beats.OrderBy(b => b.Sequence))
        {
            var actor = string.IsNullOrWhiteSpace(beat.ActorLabel) ? "Someone" : beat.ActorLabel.Trim();
            var line = beat.Kind switch
            {
                SceneBeatKind.Dialogue when !string.IsNullOrWhiteSpace(beat.Dialogue) =>
                    $"{actor} says, \"{beat.Dialogue.Trim()}\"",
                SceneBeatKind.Pose when !string.IsNullOrWhiteSpace(beat.PoseOrPosition) =>
                    $"{actor} holds this pose: {beat.PoseOrPosition.Trim()}",
                SceneBeatKind.Reaction when !string.IsNullOrWhiteSpace(beat.Reaction) =>
                    $"{actor} reacts: {beat.Reaction.Trim()}",
                SceneBeatKind.Transition when !string.IsNullOrWhiteSpace(beat.Action) =>
                    beat.Action.Trim(),
                SceneBeatKind.Narration when !string.IsNullOrWhiteSpace(beat.CaptureNote) =>
                    beat.CaptureNote.Trim(),
                _ when !string.IsNullOrWhiteSpace(beat.Action) =>
                    $"{actor} {EnsureSentenceFragment(beat.Action.Trim())}",
                _ when !string.IsNullOrWhiteSpace(beat.CaptureNote) =>
                    $"{actor}: {beat.CaptureNote.Trim()}",
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(EnsureSentence(line));
        }

        var output = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(scene.Location))
            output.AppendLine($"Location: {scene.Location.Trim()}");
        output.Append(string.Join(Environment.NewLine, lines));
        return output.ToString().Trim();
    }

    private static string EnsureSentenceFragment(string text) =>
        text.Length > 0 && char.IsUpper(text[0]) ? char.ToLowerInvariant(text[0]) + text[1..] : text;

    private static string EnsureSentence(string text) =>
        text.EndsWith('.') || text.EndsWith('!') || text.EndsWith('?') ? text : text + ".";
}
