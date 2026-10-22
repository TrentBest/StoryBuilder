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
    public void CampaignParticipantChoicesAreIndexedWithoutBeingPromotedToCanon()
    {
        var session = new StoryBuilderSession("Campaign test");
        var campaign = session.AddCampaign(new CampaignRecord { Name = "Lost Cartographers" });
        var action = session.AddParticipantAction(new ParticipantAction
        {
            CampaignId = campaign.Id,
            Description = "We leave the sealed door unopened.",
            Attribution = "Player",
            Status = CanonStatus.Proposed
        });

        Assert.Equal(CanonStatus.Proposed, action.Status);
        var usage = Assert.Single(session.Vocabulary.Search("sealed"));
        Assert.Equal(1, usage.OccurrenceCount);
        Assert.Equal("Player", usage.Occurrences[0].Attribution);
        Assert.Equal("Lost Cartographers", usage.Occurrences[0].TargetLabel);
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
