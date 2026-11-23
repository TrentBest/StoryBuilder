using StoryBuilder.Core.Models;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class NarrationDraftContractsTests
{
    [Fact]
    public void ProposalAcceptsOnlyRequestScopedBeatEventAndObserverProvenance()
    {
        var observerId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        var request = new NarrationDraftRequest
        {
            RequestId = requestId,
            ObserverId = observerId,
            AsOfEventId = eventId,
            ObserverKnowledge = new ObserverKnowledgeProjection(
                observerId,
                eventId,
                [
                    new ObserverKnownEvent(
                        eventId,
                        null,
                        [new ObserverPermittedClaim(
                            NarrationClaimMode.DirectFact,
                            Guid.NewGuid(),
                            ObserverKnowledgeKind.Witnessed,
                            "Mara",
                            "I saw it happen.",
                            null,
                            null)])
                ]),
            SourceBeats =
            [
                new NarrationSourceBeat(
                    beatId,
                    1,
                    SceneBeatKind.Action,
                    observerId,
                    "Mara",
                    "opens the gate",
                    null,
                    null,
                    null,
                    null,
                    null,
                    "Participant",
                    [eventId])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [beatId], [eventId])],
            Claims = [new NarrationEventClaim(eventId, NarrationClaimMode.DirectFact)]
        };

        NarrationDraftProposalValidator.Validate(request, proposal);
    }

    [Fact]
    public void ProposalRejectsClaimsOutsideObserverProjection()
    {
        var observerId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        var request = new NarrationDraftRequest
        {
            RequestId = requestId,
            ObserverId = observerId,
            AsOfEventId = eventId,
            ObserverKnowledge = new ObserverKnowledgeProjection(observerId, eventId, []),
            SourceBeats =
            [
                new NarrationSourceBeat(
                    beatId,
                    1,
                    SceneBeatKind.Action,
                    observerId,
                    "Mara",
                    "opens the gate",
                    null,
                    null,
                    null,
                    null,
                    null,
                    "Participant",
                    [eventId])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [beatId], [eventId])],
            Claims = [new NarrationEventClaim(eventId, NarrationClaimMode.DirectFact)]
        };

        Assert.Throws<ArgumentException>(() => NarrationDraftProposalValidator.Validate(request, proposal));
    }

    [Fact]
    public void ProposalRejectsEventProvenanceNotAssociatedWithItsCitedBeat()
    {
        var requestId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
        var otherEventId = Guid.NewGuid();
        var request = new NarrationDraftRequest
        {
            RequestId = requestId,
            SourceBeats =
            [
                new NarrationSourceBeat(
                    beatId,
                    1,
                    SceneBeatKind.Action,
                    null,
                    "Mara",
                    "opens the gate",
                    null,
                    null,
                    null,
                    null,
                    null,
                    "Participant",
                    [Guid.NewGuid()])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [beatId], [otherEventId])]
        };

        Assert.Throws<ArgumentException>(() => NarrationDraftProposalValidator.Validate(request, proposal));
    }
}
