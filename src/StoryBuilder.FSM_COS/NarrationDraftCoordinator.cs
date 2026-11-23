using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;

namespace StoryBuilder.FSM_COS;

/// <summary>
/// Coordinates request construction, optional provider invocation, and proposal validation.
/// It deliberately stops before author review, persistence, or canon acceptance.
/// </summary>
public sealed class NarrationDraftCoordinator
{
    private readonly StoryBuilderSession _session;
    private readonly INarrationDraftProvider _provider;
    private readonly NarrationDraftRequestBuilder _requestBuilder;

    public NarrationDraftCoordinator(
        StoryBuilderSession session,
        INarrationDraftProvider provider,
        NarrationDraftRequestBuilder? requestBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(provider);
        _session = session;
        _provider = provider;
        _requestBuilder = requestBuilder ?? new NarrationDraftRequestBuilder();
    }

    public async Task<NarrationDraftReview> ProposeAsync(
        Guid sceneId,
        IEnumerable<Guid> selectedBeatIds,
        Guid manuscriptId,
        Guid sectionId,
        NarrationPerspective perspective = NarrationPerspective.ThirdPersonLimited,
        string? voice = null,
        string? style = null,
        Guid? observerId = null,
        Guid? asOfEventId = null,
        bool allowOmniscientPerspective = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var providerId = string.IsNullOrWhiteSpace(_provider.ProviderId)
            ? throw new InvalidOperationException("A narration provider must declare a non-empty provider ID.")
            : _provider.ProviderId.Trim();

        var request = _requestBuilder.Build(
            _session,
            sceneId,
            selectedBeatIds,
            manuscriptId,
            sectionId,
            perspective,
            voice,
            style,
            observerId,
            asOfEventId,
            allowOmniscientPerspective);

        var proposal = await _provider.DraftAsync(request, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("The narration provider returned no proposal.");

        if (string.IsNullOrWhiteSpace(proposal.ProviderId))
        {
            proposal = proposal with { ProviderId = providerId };
        }
        else
        {
            var returnedProviderId = proposal.ProviderId.Trim();
            if (!string.Equals(returnedProviderId, providerId, StringComparison.Ordinal))
                throw new InvalidOperationException("The proposal provider attribution does not match the configured provider.");
            proposal = proposal with { ProviderId = returnedProviderId };
        }

        NarrationDraftProposalValidator.Validate(request, proposal);
        return new NarrationDraftReview(request, proposal);
    }
}

/// <summary>A validated, non-canonical proposal paired with the exact bounded request that produced it.</summary>
public sealed record NarrationDraftReview(
    NarrationDraftRequest Request,
    NarrationDraftProposal Proposal);
