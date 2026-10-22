using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class AuthoringWorkflowTests
{
    [Fact]
    public void CharacterCanStartAsASmallDraftAndChangeLater()
    {
        var builder = new CharacterBuilder();
        var character = builder.Create(templateName: "Investigator");

        Assert.Equal("Unnamed character", character.Name);
        Assert.Equal(CanonStatus.Draft, character.Status);
        Assert.Empty(character.Traits);
        Assert.Contains(builder.FindTemplate("investigator").SuggestedTraits, trait => trait == "methodical");

        var revised = character.Rename("Mara").WithDescription("A patient observer.");
        Assert.Equal("Mara", revised.Name);
        Assert.Equal("A patient observer.", revised.Description);
    }

    [Fact]
    public void VocabularySearchCombinesSynonymsButPreservesSurfaceWordsAndAttribution()
    {
        var index = new VocabularyIndex();
        var firstTarget = Guid.NewGuid();
        var secondTarget = Guid.NewGuid();
        index.AddSynonym("courageous", "brave");
        index.AddSynonym("courageous", "fearless");
        index.Record("brave", VocabularyCategory.Adjective, firstTarget, "Mara", "Traits", attribution: "Self-description");
        index.Record("fearless", VocabularyCategory.Adjective, secondTarget, "Ivo", "Dialogue", context: "A rival's opinion", attribution: "Rival");

        var usage = Assert.Single(index.Search("courageous"));
        Assert.Equal(2, usage.OccurrenceCount);
        Assert.Equal(2, usage.DistinctTargetCount);
        Assert.Contains(usage.Occurrences, occurrence => occurrence.SurfaceForm == "brave" && occurrence.Attribution == "Self-description");
        Assert.Contains(usage.Occurrences, occurrence => occurrence.SurfaceForm == "fearless" && occurrence.Attribution == "Rival");
    }

    [Fact]
    public void SceneComposerBuildsAnEditableDraftFromOrderedBeats()
    {
        var composer = new SceneComposer();
        var scene = composer.Create("The Door", "North gate", SceneCaptureMode.Claymation)
            .AddBeat(new SceneBeat { Kind = SceneBeatKind.Pose, ActorLabel = "Mara", PoseOrPosition = "left hand on the latch" })
            .AddBeat(new SceneBeat { Kind = SceneBeatKind.Dialogue, ActorLabel = "Mara", Dialogue = "Stay behind me." })
            .AddBeat(new SceneBeat { Kind = SceneBeatKind.Reaction, ActorLabel = "Ivo", Reaction = "steps back" });

        var narration = composer.ComposeNarration(scene);

        Assert.Contains("Location: North gate", narration);
        Assert.Contains("Mara holds this pose: left hand on the latch.", narration);
        Assert.Contains("Mara says, \"Stay behind me.\"", narration);
        Assert.Contains("Ivo reacts: steps back.", narration);
        Assert.Equal(SceneStatus.InProgress, scene.Status);
    }

    [Fact]
    public void SceneCanBeReorderedWithoutLosingBeats()
    {
        var scene = new SceneComposer().Create()
            .AddBeat(new SceneBeat { ActorLabel = "A", Action = "enters" })
            .AddBeat(new SceneBeat { ActorLabel = "B", Action = "turns" });

        var reordered = scene.ReorderBeats(scene.Beats.AsEnumerable().Reverse().Select(b => b.Id));

        Assert.Equal("B", reordered.Beats.OrderBy(b => b.Sequence).First().ActorLabel);
        Assert.Equal(2, reordered.Beats.Count);
    }
}
