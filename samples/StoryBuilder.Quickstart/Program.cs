using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;

var session = new StoryBuilderSession("StoryBuilder quickstart");
var trait = new CharacterTrait
{
    Term = "methodical",
    Category = "Temperament",
    Attribution = "Author",
    Status = CanonStatus.Proposed
};

var mara = session.CreateCharacter("Mara", "Investigator")
    .WithDescription("A patient observer.")
    .AddTrait(trait);
mara = session.UpdateCharacter(mara);

var vocabulary = session.Vocabulary;
vocabulary.AddSynonym("courageous", "brave");
vocabulary.Record("brave", VocabularyCategory.Adjective, mara.Id, mara.Name, "Dialogue",
    context: "A companion describes Mara.", attribution: "Companion");
var usage = vocabulary.Search("courageous").Single();

var scene = session.CreateScene("At the North Gate", "North gate", SceneCaptureMode.Claymation)
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
session.UpdateScene(scene);

Console.WriteLine("CASTING");
Console.WriteLine($"{mara.Name} — {mara.Role}");
Console.WriteLine($"Description: {mara.Description}");
Console.WriteLine($"Traits: {string.Join(", ", mara.Traits.Select(t => t.Term))}");
Console.WriteLine($"Indexed trait occurrences: {session.Vocabulary.Search("methodical").Single().OccurrenceCount}");
Console.WriteLine();
Console.WriteLine("VOCABULARY");
Console.WriteLine($"The concept '{usage.CanonicalTerm}' has {usage.OccurrenceCount} recorded occurrence(s).");
Console.WriteLine();
Console.WriteLine("SCENE DRAFT");
Console.WriteLine(session.SceneComposer.ComposeNarration(scene));
