using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryWorkspaceValidatorTests
{
    [Fact]
    public void ValidateReportsDanglingReferencesWithoutChangingWorkspace()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "Main" };
        workspace.AddTimeline(timeline);

        var missingCauseId = Guid.NewGuid();
        var missingSourceId = Guid.NewGuid();
        var storyEvent = new StoryEvent
        {
            Title = "Broken imported event",
            TimelineId = timeline.Id,
            CauseEventIds = [missingCauseId],
            SourceIds = [missingSourceId]
        };
        workspace.AddEvent(storyEvent);

        var missingEventId = Guid.NewGuid();
        var manuscript = new ManuscriptEdition
        {
            Title = "Draft",
            Sections =
            [
                new ManuscriptSection
                {
                    Title = "Chapter 1",
                    EventIds = [missingEventId]
                }
            ]
        };
        workspace.AddManuscript(manuscript);

        var issues = StoryWorkspaceValidator.Validate(workspace);

        Assert.Contains(issues, issue => issue.OwnerId == storyEvent.Id &&
            issue.Relationship == "cause event" && issue.MissingTargetId == missingCauseId);
        Assert.Contains(issues, issue => issue.OwnerId == storyEvent.Id &&
            issue.Relationship == "source" && issue.MissingTargetId == missingSourceId);
        Assert.Contains(issues, issue => issue.OwnerId == manuscript.Sections[0].Id &&
            issue.Relationship == "event" && issue.MissingTargetId == missingEventId);
        Assert.Single(workspace.Events);
        Assert.Equal(missingCauseId, Assert.Single(workspace.Events).CauseEventIds[0]);
    }

    [Fact]
    public void RelatedRecordReferencesAcceptNestedSectionAndBeatIdentities()
    {
        var workspace = new StoryWorkspace();
        var manuscript = new ManuscriptEdition
        {
            Title = "Draft",
            Sections = [new ManuscriptSection { Title = "Chapter 1" }]
        };
        var scene = new SceneRecord
        {
            Title = "Arrival",
            Beats = [new SceneBeat { Action = "Enter the harbor" }]
        };
        workspace.AddManuscript(manuscript);
        workspace.AddScene(scene);
        workspace.AddClaim(new HistoricalClaim
        {
            Statement = "The arrival was recorded in the harbor log.",
            RelatedEntityIds = [manuscript.Sections[0].Id, scene.Beats[0].Id]
        });
        workspace.AddContinuityIssue(new ContinuityIssue
        {
            Description = "Check the chapter against the scene.",
            RelatedRecordIds = [manuscript.Sections[0].Id, scene.Beats[0].Id]
        });

        Assert.Empty(StoryWorkspaceValidator.Validate(workspace));
    }

    [Fact]
    public void ValidateReportsMissingCanonicalScenePlace()
    {
        var workspace = new StoryWorkspace();
        var missingPlaceId = Guid.NewGuid();
        var scene = new SceneRecord { Title = "The arrival", PlaceId = missingPlaceId };
        workspace.AddScene(scene);

        var issue = Assert.Single(StoryWorkspaceValidator.Validate(workspace));
        Assert.Equal(nameof(SceneRecord), issue.OwnerType);
        Assert.Equal(scene.Id, issue.OwnerId);
        Assert.Equal("place", issue.Relationship);
        Assert.Equal(missingPlaceId, issue.MissingTargetId);
    }

    [Fact]
    public void SessionCreatesAndAssignsCanonicalPlaces()
    {
        var session = new StoryBuilderSession();
        var region = session.CreatePlace("The North", "A cold northern region.", "Region");
        var city = session.CreatePlace("Greyhaven", "A port city.", "City", region.Id);
        var scene = session.CreateScene("Arrival", "Harbor");
        scene = session.AssignSceneToPlace(scene.Id, city.Id);

        Assert.Equal(city.Id, scene.PlaceId);
        Assert.Contains(session.Workspace.Places, place => place.Id == region.Id);
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }

    [Fact]
    public void ValidateReportsIdentityReusedAcrossRecordTypes()
    {
        var workspace = new StoryWorkspace();
        var sharedId = Guid.NewGuid();
        workspace.AddCharacter(new CharacterProfile { Id = sharedId, Name = "Mara" });
        workspace.AddEvent(new StoryEvent { Id = sharedId, Title = "The reveal" });

        var issue = Assert.Single(StoryWorkspaceValidator.Validate(workspace));
        Assert.Equal("Workspace", issue.OwnerType);
        Assert.Equal("cross-type duplicate identity", issue.Relationship);
        Assert.Equal(sharedId, issue.MissingTargetId);
        Assert.Contains(nameof(CharacterProfile), issue.Message);
        Assert.Contains(nameof(StoryEvent), issue.Message);
    }

    [Fact]
    public void ValidateIncludesExperienceAndDesignDecisionsInIdentityChecks()
    {
        var workspace = new StoryWorkspace();
        var sharedId = Guid.NewGuid();
        workspace.Experience = new ExperienceDefinition
        {
            Id = sharedId,
            Decisions = [new DesignDecision { Id = sharedId, Decision = "Use stable identity." }]
        };

        var issues = StoryWorkspaceValidator.Validate(workspace);

        Assert.Contains(issues, issue => issue.Relationship == "duplicate identity" &&
            issue.OwnerType == nameof(DesignDecision) && issue.MissingTargetId == sharedId);
        Assert.Contains(issues, issue => issue.Relationship == "cross-type duplicate identity" &&
            issue.MissingTargetId == sharedId);
    }

    [Fact]
    public void ValidateReportsCharacterRelationshipsToMissingCharacters()
    {
        var workspace = new StoryWorkspace();
        var missingCharacterId = Guid.NewGuid();
        var character = new CharacterProfile
        {
            Name = "Mara",
            Relationships =
            [
                new CharacterRelationship
                {
                    TargetCharacterId = missingCharacterId,
                    Label = "sibling",
                    Status = CanonStatus.Accepted
                }
            ]
        };
        workspace.AddCharacter(character);

        var issues = StoryWorkspaceValidator.Validate(workspace);

        var issue = Assert.Single(issues);
        Assert.Equal(nameof(CharacterProfile), issue.OwnerType);
        Assert.Equal(character.Id, issue.OwnerId);
        Assert.Equal("related character", issue.Relationship);
        Assert.Equal(missingCharacterId, issue.MissingTargetId);
    }

    [Fact]
    public void ValidateAcceptsCharacterRelationshipsBetweenExistingCharacters()
    {
        var workspace = new StoryWorkspace();
        var sibling = new CharacterProfile { Name = "Mara" };
        var character = new CharacterProfile
        {
            Name = "Ivo",
            Relationships =
            [
                new CharacterRelationship
                {
                    TargetCharacterId = sibling.Id,
                    Label = "sibling",
                    Status = CanonStatus.Accepted
                }
            ]
        };
        workspace.AddCharacter(sibling);
        workspace.AddCharacter(character);

        Assert.Empty(StoryWorkspaceValidator.Validate(workspace));
    }

    [Fact]
    public void ValidateAcceptsLinkedAuthoringWorkflow()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The reveal");
        var manuscript = session.CreateManuscript("A book");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Chapter 1");
        session.LinkEventToManuscriptSection(manuscript.Id, section.Sections[0].Id, storyEvent.Id);

        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }
    [Fact]
    public void ValidateRejectsDuplicateStableIdsAcrossNestedManuscriptSections()
    {
        var workspace = new StoryWorkspace();
        var sharedSectionId = Guid.NewGuid();
        workspace.AddManuscript(new ManuscriptEdition
        {
            Title = "First draft",
            Sections = [new ManuscriptSection { Id = sharedSectionId, Title = "Chapter 1", Body = "First text." }]
        });
        workspace.AddManuscript(new ManuscriptEdition
        {
            Title = "Second draft",
            Sections = [new ManuscriptSection { Id = sharedSectionId, Title = "Chapter 1", Body = "Different text." }]
        });

        var issues = StoryWorkspaceValidator.Validate(workspace);

        var duplicate = Assert.Single(issues, issue =>
            issue.Relationship == "duplicate identity" &&
            issue.MissingTargetId == sharedSectionId);
        Assert.Equal(nameof(ManuscriptSection), duplicate.OwnerType);
        Assert.Contains("2 records", duplicate.Message);
        Assert.Equal(2, workspace.Manuscripts.Count);
    }

}
