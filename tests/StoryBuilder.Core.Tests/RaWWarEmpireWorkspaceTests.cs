using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using StoryBuilder.Quickstart;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class RaWWarEmpireWorkspaceTests
{
    [Fact]
    public void SourceWorkspaceContainsThirteenProvisionalHousesAndAttributedClaims()
    {
        var session = RaWWarEmpireWorkspace.Create();
        var workspace = session.Workspace;

        Assert.Equal("raWWar — Galactic Empire History", workspace.Name);
        Assert.Equal(RaWWarEmpireWorkspace.SourceUri, Assert.Single(workspace.Sources).LocationOrUri);
        Assert.Equal(13, workspace.Organizations.Count);
        Assert.Equal(23, workspace.HistoricalClaims.Count);
        Assert.All(workspace.Organizations, organization => Assert.Equal(CanonStatus.Draft, organization.Status));
        Assert.All(workspace.HistoricalClaims, claim =>
        {
            Assert.Equal(CanonStatus.Proposed, claim.Status);
            Assert.Equal("raWWar design source", claim.Attribution);
            Assert.Equal(Assert.Single(workspace.Sources).Id, Assert.Single(claim.SourceIds));
        });
        Assert.All(workspace.HistoricalClaims.Where(claim => claim.RelatedEntityIds.Count > 0), claim =>
            Assert.Contains(claim.RelatedEntityIds[0], workspace.Organizations.Select(organization => organization.Id)));
        Assert.Empty(StoryWorkspaceValidator.Validate(workspace));
    }

    [Fact]
    public void HistoricalOutlineUsesUndatedPlanningPointsRatherThanInventedEvents()
    {
        var workspace = RaWWarEmpireWorkspace.Create().Workspace;

        var timeline = Assert.Single(workspace.Timelines);
        Assert.Equal("Galactic Empire — provisional historical structure", timeline.Name);
        Assert.Empty(workspace.Events);
        Assert.Equal(17, workspace.PlanningPoints.Count);
        Assert.All(workspace.PlanningPoints, point =>
        {
            Assert.Equal(TimelinePlacementKind.Unresolved, point.PlacementKind);
            Assert.Equal(TimelinePlanningStatus.Placeholder, point.Status);
            Assert.Equal(string.Empty, point.ApproximatePlacement);
        });
        Assert.Equal(3, workspace.ContinuityIssues.Count);
        Assert.All(workspace.ContinuityIssues, issue => Assert.Equal(CanonStatus.Proposed, issue.Status));
    }

    [Fact]
    public void ProvisionalEmpireWorkspaceSurvivesBinaryPersistence()
    {
        var original = RaWWarEmpireWorkspace.Create().Workspace;
        using var output = new MemoryBinaryStream();
        new StoryWorkspaceBinaryDocument(original).Pack(output);

        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        Assert.Equal(13, restored.Workspace.Organizations.Count);
        Assert.Equal(23, restored.Workspace.HistoricalClaims.Count);
        Assert.Equal(17, restored.Workspace.PlanningPoints.Count);
        Assert.Equal(RaWWarEmpireWorkspace.SourceUri, Assert.Single(restored.Workspace.Sources).LocationOrUri);
        Assert.Empty(StoryWorkspaceValidator.Validate(restored.Workspace));
    }
}
