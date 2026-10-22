using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.WebDemo;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderObserverProjectionRendererTests
{
    [Fact]
    public void SvgShowsAcceptedKnowledgeAndOmitsUnknownEvents()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main history");
        var observer = session.CreateCharacter("Mara");
        var known = session.CreateTimelineEvent(timeline.Id, "The gate opens", "Secret world summary must not be used as observer knowledge.");
        session.AddObserverKnowledge(new ObserverKnowledgeRecord
        {
            ObserverId = observer.Id,
            EventId = known.Id,
            Kind = ObserverKnowledgeKind.Witnessed,
            AcquiredAtEventId = known.Id,
            Note = "I watched the gate open.",
            Status = CanonStatus.Accepted
        });
        session.CreateTimelineEvent(timeline.Id, "The hidden vault", "The observer does not know this.");

        var projection = new ObserverKnowledgeProjectionService().Project(session.Workspace, observer.Id, known.Id);
        var svg = new StoryBuilderObserverProjectionRenderer().Render(session.Workspace, projection);

        Assert.Contains("The gate opens", svg, StringComparison.Ordinal);
        Assert.Contains("I watched the gate open.", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("The hidden vault", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("Secret world summary", svg, StringComparison.Ordinal);
    }
}
