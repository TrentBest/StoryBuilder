using StoryBuilder.Core.Models;

namespace StoryBuilder.FSM_COS;

/// <summary>
/// Optional host-supplied capability for producing an untrusted narration proposal.
/// Implementations may call any provider, but must return source-linked output for validation.
/// </summary>
public interface INarrationDraftProvider
{
    /// <summary>Stable provider identifier used to attribute the returned proposal.</summary>
    string ProviderId { get; }

    /// <summary>Produces a proposal for the supplied bounded request; this operation must not persist canon.</summary>
    Task<NarrationDraftProposal> DraftAsync(
        NarrationDraftRequest request,
        CancellationToken cancellationToken = default);
}
