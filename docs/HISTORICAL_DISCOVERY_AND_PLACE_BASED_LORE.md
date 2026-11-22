# Historical Discovery and Place-Based Lore

A location can carry more history than the player initially knows. Treat discovery as an encounter with a layered historical record, not as a lore paragraph unlocked by proximity.

## Three separate layers

1. **World history** — what actually happened in the authored world. Represent established events as `StoryEvent` records and unresolved historical propositions as `HistoricalClaim` records. Do not manufacture event dates or causal edges to make a timeline look complete.
2. **Surviving evidence** — what remains at a location: inscriptions, structures, tools, records, biological traces, machine states, or testimony. Represent the location with a stable `PlaceRecord`; represent each source or artifact with a `SourceRecord`. Relate place-specific claims to the place ID through `HistoricalClaim.RelatedEntityIds`, and retain the source IDs that support each claim.
3. **Remembered and believed history** — what a person, institution, or culture thinks happened. Keep attribution, confidence, and canon status explicit. A source's assertion is not automatically world truth, and a character's belief is not automatically a reliable account.

## Authoring a discovery

- Create a stable place record even when its original name is unknown. Use a provisional descriptive label; do not mistake that label for the site's historical name.
- Record the site's physical and contextual description separately from interpretations of its purpose.
- Add source records for each distinct evidence item. Preserve provenance, perspective, and reliability notes; an artifact, translation, oral account, and later scholarly interpretation are different sources.
- Add separate historical claims for competing interpretations. Link each claim to the place and the evidence that supports it. Keep disputed or weakly supported claims Proposed.
- Create a discovery event only when the campaign or world chronology needs that encounter. The event is about the discovery, not proof that every interpretation of the site is true.
- Record what each observer learns, infers, is told, disputes, or forgets. Narration must not reveal facts outside that observer's permitted knowledge.

## Example pattern (illustrative, not raWWar canon)

A player enters an unnamed structure beneath a settlement. The place is recorded as **Uncatalogued Subsurface Structure**—a working label, not its original name. Three separate sources are added: a damaged inscription, a recovered navigation component, and a local oral account. Those sources support competing Proposed claims: the structure may predate the current settlement; the component may indicate a navigational function; the oral account may describe it as a place of refuge. None of these interpretations is silently promoted to fact.

The discovery event can be accepted as campaign history while the site's age, purpose, builders, and relationship to known civilizations remain unresolved. A later discovery can strengthen, overturn, or complicate the claims without erasing the original evidence or the people who interpreted it differently.

## Current StoryBuilder boundary

StoryBuilder already has stable place records with containment, source records, place-linked historical claims, event timelines, and observer-knowledge records. The current observer-knowledge record targets an event ID, however, rather than a claim or evidence item. Until that model is extended, do not force a belief about a site's age or purpose into a fabricated event merely to make it appear in an observer projection. Keep the belief as an attributed historical claim and treat direct claim/evidence knowledge projections as a future model capability.

## Design rule

**Discovery changes the observer's knowledge, not the past.** Preserve the evidence, the interpretations, their provenance, and the distinction between what happened, what survived, and what people believe happened.
