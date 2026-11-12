# Profiles integration boundary

## Status

StoryBuilder currently has **no Profiles package dependency and no authenticated author adapter**. The WebDemo registers no authentication or identity-resolution service. Its current `ManuscriptEdition.Author` and `PublishedBookEdition.Author` values are plain display strings: they are rendered into Markdown and persisted as story content. They are not authenticated identities, Profile IDs, persona IDs, proof of authorship, or authorization grants.

Do not interpret the existing free-text author field as a trusted host identity. Until a host supplies a validated author context, a host must not use client-entered author text to establish ownership, editing rights, campaign participation, or publication authority.

## Ownership and vocabulary

| Concept | Owner | Meaning |
| --- | --- | --- |
| Authenticated principal | Host authentication layer | The principal established by the host's authentication mechanism. |
| Canonical Profile | Profiles and trusted identity infrastructure | The entity identity resolved by a trusted host mapping. |
| Selected author persona | Profiles plus trusted host adapter | The context-specific publisher/creator representation selected by the authenticated principal. |
| Story author attribution | StoryBuilder | The story-domain attribution attached to a manuscript or immutable edition; it may contain an approved display projection. |
| Fictional character | StoryBuilder | A story-world entity represented by `CharacterProfile`; never a Profiles identity by implication. |

A `CharacterProfile.Id` identifies a fictional/story-world character. It must not be overloaded with a ProfileId, external identity subject, account ID, or author/persona identifier. Character names, traits, pronouns, species, relationships, and notes remain story data.

## Required host adapter contract

A future authenticated host integration should follow this order:

1. Authenticate the request and obtain the principal from trusted server-side authentication state. Never accept a principal, ProfileId, persona ID, external subject, or claim bundle from an authoring form as authority.
2. Resolve the principal-to-Profile mapping through trusted identity infrastructure. Profiles identity links and external identity bindings remain private to that infrastructure.
3. Resolve the selected persona and verify that it belongs to, or is explicitly authorized for, the authenticated principal. An unmapped, forged, revoked, unavailable, or ambiguous mapping fails closed.
4. Obtain a representation for the intended observer using trusted observer and group context. Do not pass a complete `Profile`, raw claims, `ProfileIdentityLink`, `ProfileExternalIdentityBinding`, credentials, access records, or private identity metadata to StoryBuilder.
5. Map only explicitly allowlisted publication fields (for example, an approved display name) into StoryBuilder's story-facing author attribution. Do not treat avatar, display name, or any other disclosure as permission to edit or publish.
6. Independently authorize the requested operation: workspace access, draft editing, campaign participation, and publication are separate decisions. Disclosure is not authorization.
7. Record the host's audit event in host-owned infrastructure. Do not serialize private identity resolution or access records into a StoryBuilder workspace or publication.

The adapter should expose a small host-owned value/port, not make StoryBuilder.Core, StoryBuilder.Persistence, StoryBuilder.MicroBundle, or standalone consumers reference Profiles. A host may implement that port with Profiles and pass only the approved projection into StoryBuilder. If a future requirement proves that a stable opaque author/persona reference must be retained by StoryBuilder, define its exact purpose, lifecycle, revocation behavior, and serialization policy before adding a domain field or changing the binary format.

## Publication and persistence rules

- `ManuscriptEdition.Author` is currently a plain-text display field.
- `PublishedBookEdition.Author` and the rendered Markdown freeze the display string present at publication time.
- These fields must contain only the host-approved author display projection when used in an authenticated publishing flow. They must never contain a canonical/private Profile record, raw claims, identity-link data, external provider subject, credentials, or authorization state.
- Public and restricted editions must be composed from explicit allowlisted author projections appropriate to the target audience. Do not reuse a privileged/internal projection for public export.
- A published display name is historical attribution, not a live authorization check. Revocation or persona changes affect future authorized operations and exports; they do not silently rewrite immutable editions.
- StoryBuilder's current binary workspace schema has no Profile identity or persona fields. Preserve that independence.

## Current implementation gap

The domain distinction is present in the model shape: `CharacterProfile` is fictional story data, and author fields are strings. The dependency boundary is also clean: `StoryBuilder.Core.csproj` has no package references, and the WebDemo currently has no Profiles integration. However, the trusted host adapter, principal/persona validation, observer-specific projection mapping, fail-closed revoked/unmapped tests, and audience-specific publication authorization are **not implemented**. Do not claim those acceptance criteria are complete until an authenticated host supplies the adapter and integration tests exercise it.

## Verification checklist for a future host

- [ ] A client-supplied or forged principal/ProfileId/persona ID cannot select an author identity.
- [ ] Unknown, unmapped, revoked, and mismatched principal/persona mappings fail closed without changing the workspace.
- [ ] The host obtains an observer-specific Profiles representation using trusted observer context.
- [ ] Public and restricted publication paths each use their own explicit allow-list.
- [ ] Private claims, identity links, external identity bindings, credentials, and access records are absent from workspace files and published artifacts.
- [ ] Editing, campaign participation, and publication authorization are tested independently of disclosure.
- [ ] Fictional character IDs cannot be interpreted as Profile IDs.
- [ ] Core, persistence, MicroBundle, and standalone consumers still run without Profiles installed.
