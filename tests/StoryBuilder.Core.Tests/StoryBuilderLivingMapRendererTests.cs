using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.WebDemo;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderLivingMapRendererTests
{
    [Fact]
    public void LivingSvgShowsCurrentCharactersAndVocabularyRatherThanStaticMockData()
    {
        var session = new StoryBuilderSession("Living map test");
        var renderer = new StoryBuilderLivingMapRenderer();

        var emptySvg = renderer.Render(session);
        Assert.DoesNotContain("Mara", emptySvg);

        var character = session.CreateCharacter("Mara", "Cartographer")
            .AddTrait(new CharacterTrait { Term = "observant", Attribution = "Rival" });
        session.UpdateCharacter(character);

        var populatedSvg = renderer.Render(session);
        Assert.Contains("Mara", populatedSvg);
        Assert.Contains("Cartographer", populatedSvg);
        Assert.Contains("observant", populatedSvg);
        Assert.Contains("MOST-USED WORDS", populatedSvg);
        Assert.NotEqual(emptySvg, populatedSvg);
    }
}
