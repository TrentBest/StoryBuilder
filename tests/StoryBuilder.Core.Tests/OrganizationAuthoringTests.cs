using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class OrganizationAuthoringTests
{
    [Fact]
    public void OrganizationsAreDistinctEventParticipantsAndPersistAcrossRoundTrip()
    {
        var session = new StoryBuilderSession("Organization authoring");
        var house = session.CreateOrganization("House Veyr", "House", "A provisional political institution.");
        var timeline = session.CreateTimeline("Imperial history");
        var founding = session.CreateTimelineEvent(timeline.Id, "The compact is proposed");
        session.UpdateTimelineEvent(founding with { OrganizationParticipantIds = [house.Id] });

        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        var restoredHouse = Assert.Single(restored.Workspace.Organizations);
        Assert.Equal(house.Id, restoredHouse.Id);
        Assert.Equal("House Veyr", restoredHouse.Name);
        Assert.Equal(house.Id, Assert.Single(Assert.Single(restored.Workspace.Events).OrganizationParticipantIds));
    }

    [Fact]
    public void ValidatorReportsMissingOrganizationParticipantsAndCrossTypeIdentityReuse()
    {
        var workspace = new StoryWorkspace();
        var sharedId = Guid.NewGuid();
        workspace.AddOrganization(new OrganizationRecord { Id = sharedId, Name = "House Veyr" });
        workspace.AddEvent(new StoryEvent { OrganizationParticipantIds = [Guid.NewGuid()] });
        workspace.AddCharacter(new CharacterProfile { Id = sharedId, Name = "Mara" });

        var issues = StoryWorkspaceValidator.Validate(workspace);
        Assert.Contains(issues, issue => issue.Relationship == "participant organization");
        Assert.Contains(issues, issue => issue.Relationship == "cross-type duplicate identity" &&
            issue.MissingTargetId == sharedId);
    }

    [Fact]
    public void OrganizationUpdatesRefreshVocabulary()
    {
        var session = new StoryBuilderSession();
        var organization = session.CreateOrganization("House Veyr", "House");
        session.UpdateOrganization(organization with { Name = "Veyr Imperial House" });

        Assert.Empty(session.Vocabulary.Search("House Veyr"));
        Assert.Contains(session.Vocabulary.Search("Imperial"), occurrence => occurrence.OccurrenceCount > 0);
    }
}
