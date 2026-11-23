using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class SceneComposerBeatTests
{
    [Fact]
    public void ComposeNarrationBeatMatchesTheCorrespondingWholeSceneLine()
    {
        var composer = new SceneComposer();
        var beat = new SceneBeat
        {
            Kind = SceneBeatKind.Action,
            ActorLabel = "Mara",
            Action = "opens the gate"
        };
        var scene = new SceneRecord
        {
            Title = "The North Gate",
            Beats = [beat]
        };

        Assert.Equal("Mara opens the gate.", composer.ComposeNarrationBeat(beat));
        Assert.Equal(composer.ComposeNarrationBeat(beat), composer.ComposeNarration(scene));
    }

    [Fact]
    public void ComposeNarrationBeatPreservesDialogueAndReturnsEmptyForAnUnwrittenBeat()
    {
        var composer = new SceneComposer();
        var dialogue = new SceneBeat
        {
            Kind = SceneBeatKind.Dialogue,
            ActorLabel = "Mara",
            Dialogue = "Stay behind me."
        };
        var empty = new SceneBeat { Kind = SceneBeatKind.Action };

        Assert.Equal("Mara says, \"Stay behind me.\"", composer.ComposeNarrationBeat(dialogue));
        Assert.Equal(string.Empty, composer.ComposeNarrationBeat(empty));
    }
}
