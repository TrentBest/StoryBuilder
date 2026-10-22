using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class NarratorPremiseTests
{
    [Fact]
    public void NarratorCapturesAnIdeaAsARevisableProposalAndIndexesItsWords()
    {
        var session = new StoryBuilderSession("Narrator test");

        var premise = session.CreatePremise(
            "A cartographer finds Earth on a forgotten star map.",
            scale: "Connected series");

        Assert.Equal(CanonStatus.Proposed, premise.Status);
        Assert.Contains("cartographer", premise.Title, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("Connected series", premise.Scale);
        Assert.Single(session.Workspace.Premises);
        Assert.Contains(session.Vocabulary.Search("cartographer"), usage =>
            usage.Occurrences.Any(occurrence => occurrence.TargetId == premise.Id));

        var revised = session.UpdatePremise(premise with
        {
            Title = "The Forgotten Planet",
            Vision = "A pilot searches for a lost planet."
        });

        Assert.Equal("The Forgotten Planet", revised.Title);
        Assert.Empty(session.Vocabulary.Search("cartographer"));
        Assert.Contains(session.Vocabulary.Search("pilot"), usage =>
            usage.Occurrences.Any(occurrence => occurrence.TargetId == premise.Id));
    }
}
