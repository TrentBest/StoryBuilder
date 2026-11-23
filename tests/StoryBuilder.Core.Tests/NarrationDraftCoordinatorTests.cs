using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.FSM_COS;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class NarrationDraftCoordinatorTests
{
    [Fact]
    public async Task ProposeAsyncBuildsBoundedRequestValidatesProposalAndDoesNotSaveCanon()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(request => new NarrationDraftProposal
        {
            RequestId = request.RequestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [fixture.Beat.Id], [fixture.Event.Id])],
            Claims = [new NarrationEventClaim(fixture.Event.Id, NarrationClaimMode.DirectFact)]
        });
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);
        var changes = 0;
        fixture.Session.Changed += () => changes++;

        var review = await coordinator.ProposeAsync(
            fixture.Scene.Id,
            [fixture.Beat.Id],
            fixture.Manuscript.Id,
            fixture.Section.Id,
            voice: "restrained",
            style: "historical");

        Assert.NotNull(provider.LastRequest);
        Assert.Equal(fixture.Beat.Id, provider.LastRequest!.SourceBeats.Single().BeatId);
        Assert.Equal("restrained", review.Request.Voice);
        Assert.Equal("historical", review.Request.Style);
        Assert.Equal("test-provider", review.Proposal.ProviderId);
        Assert.Equal(review.Request.RequestId, review.Proposal.RequestId);
        Assert.Equal(0, changes);
        Assert.Empty(fixture.Session.Workspace.Manuscripts.Single().Sections.Single().Body);
    }

    [Fact]
    public async Task ProposeAsyncRejectsProviderAttributionThatDoesNotMatchConfiguredProvider()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(request => new NarrationDraftProposal
        {
            RequestId = request.RequestId,
            ProviderId = "different-provider",
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [fixture.Beat.Id], [fixture.Event.Id])]
        });
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);

        await Assert.ThrowsAsync<InvalidOperationException>(() => coordinator.ProposeAsync(
            fixture.Scene.Id,
            [fixture.Beat.Id],
            fixture.Manuscript.Id,
            fixture.Section.Id));
    }

    [Fact]
    public async Task ProposeAsyncRejectsProposalForDifferentRequest()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(_ => new NarrationDraftProposal
        {
            RequestId = Guid.NewGuid(),
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [fixture.Beat.Id], [fixture.Event.Id])]
        });
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);

        await Assert.ThrowsAsync<ArgumentException>(() => coordinator.ProposeAsync(
            fixture.Scene.Id,
            [fixture.Beat.Id],
            fixture.Manuscript.Id,
            fixture.Section.Id));
    }

    [Fact]
    public async Task ProposeAsyncHonorsCancellationBeforeInvokingProvider()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(_ => throw new InvalidOperationException("Provider should not be called."));
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => coordinator.ProposeAsync(
            fixture.Scene.Id,
            [fixture.Beat.Id],
            fixture.Manuscript.Id,
            fixture.Section.Id,
            cancellationToken: cancellation.Token));

        Assert.False(provider.Called);
    }

    [Fact]
    public async Task AcceptNarrationDraftPersistsTextEventLinksAndImmutableProvenance()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(request => new NarrationDraftProposal
        {
            RequestId = request.RequestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [fixture.Beat.Id], [fixture.Event.Id])],
            Claims = [new NarrationEventClaim(fixture.Event.Id, NarrationClaimMode.DirectFact)]
        });
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);
        var review = await coordinator.ProposeAsync(
            fixture.Scene.Id, [fixture.Beat.Id], fixture.Manuscript.Id, fixture.Section.Id);

        var changes = 0;
        fixture.Session.Changed += () => changes++;
        var accepted = fixture.Session.AcceptNarrationDraft(review.Request, review.Proposal, "Author");
        var section = accepted.Sections.Single();

        Assert.Equal("Mara opened the gate.", section.Body);
        Assert.Contains(fixture.Event.Id, section.EventIds);
        var provenance = Assert.Single(section.NarrationAcceptances);
        Assert.Equal(review.Request.RequestId, provenance.RequestId);
        Assert.Equal("Author", provenance.Author);
        Assert.Equal("test-provider", provenance.ProviderId);
        Assert.Equal(review.Proposal.Text, provenance.AcceptedText);
        Assert.Equal(fixture.Beat.Id, Assert.Single(Assert.Single(provenance.Segments).SourceBeatIds));
        Assert.Equal(fixture.Event.Id, Assert.Single(provenance.Claims).EventId);
        Assert.Equal(1, changes);
    }

    [Fact]
    public async Task AcceptNarrationDraftRejectsWhenSourceBeatChangedAfterProposal()
    {
        var fixture = CreateFixture();
        var provider = new TestProvider(request => new NarrationDraftProposal
        {
            RequestId = request.RequestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [fixture.Beat.Id], [fixture.Event.Id])]
        });
        var coordinator = new NarrationDraftCoordinator(fixture.Session, provider);
        var review = await coordinator.ProposeAsync(
            fixture.Scene.Id, [fixture.Beat.Id], fixture.Manuscript.Id, fixture.Section.Id);
        var currentScene = fixture.Session.Workspace.Scenes.Single();
        var changedBeat = fixture.Beat with { Action = "bars the gate" };
        fixture.Session.UpdateScene(currentScene with { Beats = [changedBeat] });

        Assert.Throws<InvalidOperationException>(() =>
            fixture.Session.AcceptNarrationDraft(review.Request, review.Proposal, "Author"));
        var section = fixture.Session.Workspace.Manuscripts.Single().Sections.Single();
        Assert.Empty(section.Body);
        Assert.Empty(section.NarrationAcceptances);
    }

    private static Fixture CreateFixture()
    {
        var session = new StoryBuilderSession();
        var timeline = session.CreateTimeline("Main");
        var storyEvent = session.CreateTimelineEvent(timeline.Id, "The gate opens");
        var scene = session.CreateScene("At the north gate");
        var beat = new SceneBeat
        {
            Sequence = 1,
            Kind = SceneBeatKind.Action,
            ActorLabel = "Mara",
            Action = "opens the gate",
            EventIds = [storyEvent.Id]
        };
        session.UpdateScene(scene with { Beats = [beat] });
        var manuscript = session.CreateManuscript("The Chronicle");
        manuscript = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "The North Gate");
        var section = manuscript.Sections.Single();
        return new Fixture(session, scene, beat, storyEvent, manuscript, section);
    }

    private sealed record Fixture(
        StoryBuilderSession Session,
        SceneRecord Scene,
        SceneBeat Beat,
        StoryEvent Event,
        ManuscriptEdition Manuscript,
        ManuscriptSection Section);

    private sealed class TestProvider(Func<NarrationDraftRequest, NarrationDraftProposal> createProposal)
        : INarrationDraftProvider
    {
        public string ProviderId { get; set; } = "test-provider";
        public bool Called { get; private set; }
        public NarrationDraftRequest? LastRequest { get; private set; }

        public Task<NarrationDraftProposal> DraftAsync(
            NarrationDraftRequest request,
            CancellationToken cancellationToken = default)
        {
            Called = true;
            LastRequest = request;
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(createProposal(request));
        }
    }
}
