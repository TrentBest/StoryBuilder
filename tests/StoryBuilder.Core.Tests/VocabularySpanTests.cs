using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class VocabularySpanTests
{
    [Fact]
    public void RecordTextPreservesSurfaceWordOffsetsAndLengths()
    {
        var index = new VocabularyIndex();
        var source = "Mara was brave, and brave-minded when the station shook.";

        var occurrences = index.RecordText(
            source,
            VocabularyCategory.Adjective,
            Guid.NewGuid(),
            "Mara",
            nameof(CharacterProfile.Description),
            attribution: "Narrator");

        var brave = occurrences.Where(item => item.SurfaceForm == "brave").ToArray();
        var compound = Assert.Single(occurrences, item => item.SurfaceForm == "brave-minded");

        Assert.Equal(2, brave.Length);
        Assert.Equal(source.IndexOf("brave", StringComparison.Ordinal), brave[0].StartIndex);
        Assert.Equal(5, brave[0].Length);
        Assert.Equal("brave", source.Substring(brave[0].StartIndex, brave[0].Length));
        Assert.Equal(source.IndexOf("brave-minded", StringComparison.Ordinal), compound.StartIndex);
        Assert.Equal("brave-minded", source.Substring(compound.StartIndex, compound.Length));
        Assert.All(occurrences, item => Assert.Equal("Narrator", item.Attribution));
    }
}
