using StoryBuilder.Core.Models;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class WorkspaceTests
{
    [Fact]
    public void WorkspaceKeepsBuildRecordAndFictionalClaimsInSeparateCollections()
    {
        var workspace = new StoryWorkspace
        {
            Experience = new ExperienceDefinition
            {
                Name = "StoryBuilder",
                Purpose = "Compose stories and capture their creation."
            }
        };
        var claim = new HistoricalClaim
        {
            Statement = "The location of humanity's origin world is unknown.",
            Status = CanonStatus.Proposed
        };

        workspace.AddClaim(claim);

        Assert.Equal("StoryBuilder", workspace.Experience.Name);
        Assert.Empty(workspace.Characters);
        Assert.Single(workspace.HistoricalClaims);
        Assert.NotEqual(workspace.Experience.Id, claim.Id);
    }

    [Fact]
    public void WorkspaceRejectsDuplicateIdentityWithinARecordCollection()
    {
        var workspace = new StoryWorkspace();
        var character = new CharacterProfile { Id = Guid.NewGuid(), Name = "Mara" };
        workspace.AddCharacter(character);

        Assert.Throws<InvalidOperationException>(() => workspace.AddCharacter(character));
    }
}
