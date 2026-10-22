using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;

var characterBuilder = new CharacterBuilder();
var mara = characterBuilder.Create("Mara", "Investigator")
    .WithDescription("A patient observer.")
    .AddTrait(new CharacterTrait
    {
        Term = "methodical",
        Category = "Temperament",
        Attribution = "Author",
        Status = CanonStatus.Proposed
    });

var vocabulary = new VocabularyIndex();
vocabulary.AddSynonym("courageous", "brave");
vocabulary.AddSynonym("courageous", "fearless");
vocabulary.Record("brave", VocabularyCategory.Adjective, mara.Id, mara.Name, "Traits", attribution: "Author");
var usage = vocabulary.Search("courageous").Single();

var composer = new SceneComposer();
var scene = composer.Create("At the North Gate", "North gate", SceneCaptureMode.Claymation)
    .AddBeat(new SceneBeat
    {
        Kind = SceneBeatKind.Pose,
        ActorId = mara.Id,
        ActorLabel = mara.Name,
        PoseOrPosition = "one hand resting on the latch"
    })
    .AddBeat(new SceneBeat
    {
        Kind = SceneBeatKind.Dialogue,
        ActorId = mara.Id,
        ActorLabel = mara.Name,
        Dialogue = "Stay behind me."
    })
    .AddBeat(new SceneBeat
    {
        Kind = SceneBeatKind.Action,
        ActorId = mara.Id,
        ActorLabel = mara.Name,
        Action = "opens the gate slowly"
    });

Console.WriteLine("CASTING");
Console.WriteLine($"{mara.Name} — {mara.Role}");
Console.WriteLine($"Description: {mara.Description}");
Console.WriteLine($"Traits: {string.Join(", ", mara.Traits.Select(t => t.Term))}");
Console.WriteLine();
Console.WriteLine("VOCABULARY");
Console.WriteLine($"The concept '{usage.CanonicalTerm}' has {usage.OccurrenceCount} recorded occurrence(s).");
Console.WriteLine();
Console.WriteLine("SCENE DRAFT");
Console.WriteLine(composer.ComposeNarration(scene));
