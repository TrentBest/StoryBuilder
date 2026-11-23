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
    public void ProposalRejectsTextWithoutSourceBeatCoverage()
    {
        var requestId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
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
                    [])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate."
        };

        Assert.Throws<ArgumentException>(() => NarrationDraftProposalValidator.Validate(request, proposal));
    }

    [Fact]
    public void ProposalRejectsNullDeserializedClaims()
    {
        var requestId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
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
                    [])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [beatId], [])],
            Claims = new NarrationEventClaim[] { null! }
        };

        Assert.Throws<ArgumentException>(() => NarrationDraftProposalValidator.Validate(request, proposal));
    }

    [Fact]
    public void ProposalRejectsEventClaimNotLinkedToAProvenanceSegment()
    {
        var requestId = Guid.NewGuid();
        var beatId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
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
                    [eventId])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = "Mara opened the gate.",
            Segments = [new NarrationDraftSegment(0, 21, [beatId], [])],
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
    [Fact]
    public void ProposalAcceptsSeparateTextRangesForTheirOwnSourceBeatsAndEvents()
    {
        var requestId = Guid.NewGuid();
        var firstBeatId = Guid.NewGuid();
        var secondBeatId = Guid.NewGuid();
        var firstEventId = Guid.NewGuid();
        var secondEventId = Guid.NewGuid();
        var firstText = "Mara opens the gate.";
        var secondText = "The guard steps aside.";
        var separator = Environment.NewLine + Environment.NewLine;
        var combinedText = firstText + separator + secondText;
        var secondStart = firstText.Length + separator.Length;
        var request = new NarrationDraftRequest
        {
            RequestId = requestId,
            SourceBeats =
            [
                new NarrationSourceBeat(firstBeatId, 1, SceneBeatKind.Action, null, "Mara",
                    "opens the gate", null, null, null, null, null, "Participant", [firstEventId]),
                new NarrationSourceBeat(secondBeatId, 2, SceneBeatKind.Action, null, "Guard",
                    "steps aside", null, null, null, null, null, "Participant", [secondEventId])
            ]
        };
        var proposal = new NarrationDraftProposal
        {
            RequestId = requestId,
            Text = combinedText,
            Segments =
            [
                new NarrationDraftSegment(0, firstText.Length, [firstBeatId], [firstEventId]),
                new NarrationDraftSegment(secondStart, secondText.Length, [secondBeatId], [secondEventId])
            ]
        };

        NarrationDraftProposalValidator.Validate(request, proposal);
    }

}
