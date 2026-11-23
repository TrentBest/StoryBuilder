using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
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
    public void CampaignRejectsReferencesToMissingCharactersScenesAndEvents()
    {
        var workspace = new StoryWorkspace();
        var missingId = Guid.NewGuid();

        Assert.Throws<KeyNotFoundException>(() => workspace.AddCampaign(new CampaignRecord
        {
            ParticipantCharacterIds = [missingId]
        }));
        Assert.Throws<KeyNotFoundException>(() => workspace.AddCampaign(new CampaignRecord
        {
            SceneIds = [missingId]
        }));
        Assert.Throws<KeyNotFoundException>(() => workspace.AddCampaign(new CampaignRecord
        {
            EventIds = [missingId]
        }));
    }

    [Fact]
    public void ParticipantActionMustBelongToAnExistingCampaignAndReferenceExistingRecords()
    {
        var workspace = new StoryWorkspace();
        var missingId = Guid.NewGuid();

        Assert.Throws<KeyNotFoundException>(() => workspace.AddParticipantAction(new ParticipantAction
        {
            CampaignId = missingId
        }));

        var campaign = new CampaignRecord();
        workspace.AddCampaign(campaign);
        Assert.Throws<KeyNotFoundException>(() => workspace.AddParticipantAction(new ParticipantAction
        {
            CampaignId = campaign.Id,
            CharacterId = missingId
        }));
        Assert.Throws<KeyNotFoundException>(() => workspace.AddParticipantAction(new ParticipantAction
        {
            CampaignId = campaign.Id,
            ResultingEventId = missingId
        }));
    }

    [Fact]
    [Fact]
    public void AdventureQuestAndObjectiveMustRespectCampaignOwnership()
    {
        var session = new StoryBuilderSession("Campaign hierarchy");
        var campaign = session.AddCampaign(new CampaignRecord { Name = "The Long Road" });
        var other = session.AddCampaign(new CampaignRecord { Name = "The Other Road" });
        var adventure = session.CreateAdventure(campaign.Id, "The Broken Gate");
        var quest = session.CreateQuest(campaign.Id, adventure.Id, "Find the key");
        var objective = session.CreateQuestObjective(quest.Id, "Locate the keeper");
        Assert.Equal(campaign.Id, adventure.CampaignId);
        Assert.Equal(adventure.Id, quest.AdventureId);
        Assert.Equal(quest.Id, objective.QuestId);
        Assert.Equal(campaign.Id, objective.CampaignId);
        Assert.Throws<KeyNotFoundException>(() => session.CreateQuest(other.Id, adventure.Id, "Cross-campaign quest"));
    }

    [Fact]
    public void QuestObjectivesRequireExistingQuestAndPositiveSequence()
    {
        var workspace = new StoryWorkspace();
        var campaign = new CampaignRecord();
        workspace.AddCampaign(campaign);
        Assert.Throws<KeyNotFoundException>(() => workspace.AddQuestObjective(new QuestObjectiveRecord { CampaignId = campaign.Id, QuestId = Guid.NewGuid() }));
        var quest = new QuestRecord { CampaignId = campaign.Id };
        workspace.AddQuest(quest);
        Assert.Throws<ArgumentOutOfRangeException>(() => workspace.AddQuestObjective(new QuestObjectiveRecord { CampaignId = campaign.Id, QuestId = quest.Id, Sequence = 0 }));
    }

    public void WorkspaceRejectsDuplicateIdentityWithinARecordCollection()
    {
        var workspace = new StoryWorkspace();
        var character = new CharacterProfile { Id = Guid.NewGuid(), Name = "Mara" };
        workspace.AddCharacter(character);

        Assert.Throws<InvalidOperationException>(() => workspace.AddCharacter(character));
    }
}
