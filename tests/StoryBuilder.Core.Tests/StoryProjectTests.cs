using TheSingularityWorkshop.StoryBuilder;
using Xunit;

namespace TheSingularityWorkshop.StoryBuilder.Tests;

public sealed class StoryProjectTests
{
    [Fact]
    public void EventLinksToReusablePassageAndKeepsStableIdentity()
    {
        var project = StoryProject.Create("Test");
        project.Manuscript = "The station lights went dark. Then the alarms began.";

        var first = project.AddEvent("Blackout", passageText: "The station lights went dark.");
        var second = project.AddEvent("Alarm", passageText: "The station lights went dark.");

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(first.PassageId, second.PassageId);
        Assert.Single(project.Passages);
    }

    [Fact]
    public void EventCannotLinkToTextOutsideManuscript()
    {
        var project = StoryProject.Create();
        project.Manuscript = "A quiet beginning.";

        Assert.Throws<ArgumentException>(() =>
            project.AddEvent("Impossible", passageText: "The moon exploded."));
    }

    [Fact]
    public void ChronologyPlacesDatedEventsBeforeUndatedEvents()
    {
        var project = StoryProject.Create();
        var later = project.AddEvent("Later", occurredAt: DateTimeOffset.Parse("2200-01-02T00:00:00Z"));
        var earlier = project.AddEvent("Earlier", occurredAt: DateTimeOffset.Parse("2200-01-01T00:00:00Z"));
        var undated = project.AddEvent("Unplaced");

        var chronology = project.GetChronology();

        Assert.Equal(new[] { earlier.Id, later.Id, undated.Id }, chronology.Select(item => item.Id));
    }

    [Fact]
    public async Task LocalStoreRoundTripsProjectAndStableEventIds()
    {
        var folder = Path.Combine(Path.GetTempPath(), "StoryBuilderTests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "story.storybuilder");
        try
        {
            var project = StoryProject.Create("Round trip");
            project.Premise = "A premise.";
            project.Manuscript = "A passage.";
            var item = project.AddEvent("Opening", passageText: "A passage.");
            var store = new LocalProjectStore();

            await store.SaveAsync(path, project);
            var reopened = await store.LoadAsync(path);

            Assert.Equal(project.ProjectId, reopened.ProjectId);
            Assert.Equal(project.Title, reopened.Title);
            Assert.Equal(item.Id, Assert.Single(reopened.Timeline).Id);
            Assert.Equal(item.PassageId, Assert.Single(reopened.Passages).Id);
        }
        finally
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, recursive: true);
        }
    }
}
