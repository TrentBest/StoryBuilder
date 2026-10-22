using StoryBuilder.Core.Models;
using StoryBuilder.StoryForge;
using Xunit;

namespace StoryBuilder.StoryForge.Tests;

public sealed class StoryWorkspaceTemporalProjectionTests
{
    [Fact]
    public void ProjectionUsesCanonicalWorkspaceIdsAndRetainsSourceProvenance()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "World History" };
        var source = new SourceRecord
        {
            Title = "Chapter One",
            LocationOrUri = "manuscript:chapter-1"
        };
        var participantId = Guid.NewGuid();
        var storyEvent = new StoryBuilder.Core.Models.StoryEvent
        {
            Title = "First Contact",
            Summary = "The two crews meet.",
            InWorldDateOrPeriod = "Year 42",
            TimelineId = timeline.Id,
            ParticipantIds = [participantId],
            SourceIds = [source.Id]
        };

        workspace.AddTimeline(timeline);
        workspace.AddSource(source);
        workspace.AddEvent(storyEvent);

        var engine = StoryWorkspaceTemporalProjection.CreateEngine(workspace, item =>
            item.InWorldDateOrPeriod == "Year 42" ? 42m : null);

        var result = engine.QueryAt(new TemporalQuery(new StoryTime(timeline.Id.ToString("D"), 42m),
            participantId.ToString("D")));

        var projected = Assert.Single(result.Events);
        Assert.Equal(storyEvent.Id.ToString("D"), projected.Id);
        Assert.Equal("First Contact", projected.Title);
        Assert.Equal("The two crews meet.", projected.Description);
        Assert.Equal(participantId.ToString("D"), Assert.Single(projected.ParticipantIds));
        var provenance = Assert.Single(projected.Provenance);
        Assert.Equal(source.Id.ToString("D"), provenance.SourceId);
        Assert.Equal("Chapter One", provenance.Description);
        Assert.Equal("manuscript:chapter-1", provenance.Locator);
        Assert.Equal(StoryCertainty.Proposed, projected.Certainty);
    }

    [Fact]
    public void ProjectionSkipsEventsWithoutResolvableCoordinatesOrValidTimelines()
    {
        var workspace = new StoryWorkspace();
        var timeline = new TimelineDefinition { Name = "World History" };
        workspace.AddTimeline(timeline);
        workspace.AddEvent(new StoryBuilder.Core.Models.StoryEvent
        {
            Title = "Unknown date",
            TimelineId = timeline.Id,
            InWorldDateOrPeriod = "Sometime later"
        });
        workspace.AddEvent(new StoryBuilder.Core.Models.StoryEvent
        {
            Title = "Unassigned event",
            InWorldDateOrPeriod = "Year 42"
        });

        var engine = StoryWorkspaceTemporalProjection.CreateEngine(workspace, _ => null);

        var result = engine.QueryAt(new TemporalQuery(new StoryTime(timeline.Id.ToString("D"), 42m)));
        Assert.Empty(result.Events);
    }
}
