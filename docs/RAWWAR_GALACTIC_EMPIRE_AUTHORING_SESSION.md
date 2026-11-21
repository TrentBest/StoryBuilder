# raWWar Galactic Empire — StoryBuilder Authoring Session

**Status:** Working import plan; not canon  
**Consumer:** raWWar / Galactic War  
**StoryBuilder role:** Historical and narrative coherence  
**Game-design owner:** raWWar  
**Source baseline:** [Empire History & Faction Origins](https://github.com/TrentBest/raWWar/blob/development/docs/EMPIRE_HISTORY_AND_FACTION_ORIGINS.md)

## 1. Purpose and ownership

This document coordinates the first StoryBuilder authoring pass for raWWar. It is not a replacement for the raWWar design documents and does not promote their contents to canon.

- raWWar defines the game, faction meaning, mechanics, player activities, political systems, and intended experience.
- StoryBuilder organizes historical records, chronology, causes, consequences, claims, sources, perspectives, and continuity.
- A source statement, imported record, or generated proposal is not accepted canon merely because it has been recorded.
- If the two repositories disagree, preserve the discrepancy and request an owner decision. Do not silently reconcile it.

## 2. Source handling

Create a source record for each imported raWWar document, preserving:
- exact repository path and branch/ref used;
- retrieval/revision identity when available;
- authoring perspective and intended authority;
- what the document explicitly states versus what it leaves unresolved.

Treat the source document as evidence that raWWar currently proposes or records a particular design—not as independent proof that every in-world claim is objectively true.

For each historical assertion, preserve three distinct questions:
1. What does the current authoring source claim happened?
2. What evidence or in-world records survived?
3. What do different people, institutions, and factions believe happened?

## 3. Organization records required

The raWWar source describes thirteen Houses as distinct political/civilizational actors. They must be represented as organizations, not disguised as characters or places.

| Source-defined House | Source-described historical pressure / disposition |
|---|---|
| House Veyr | Stability and preservation of order |
| House Cael | Imperial administration |
| House Oris | Industrial power |
| House Sere | Independent knowledge |
| House Avar | Cultural preservation |
| House Talan | Merchant independence |
| House Merid | Political consent and representation |
| House Rhen | Frontier self-reliance |
| House Solenne | Civilian welfare |
| House Kharos | Professional military competence |
| House Nyr | Local autonomy |
| House Voss | Revolution |
| House Draven | Uncompromising resistance |

These are source summaries, not complete faction definitions. Do not infer that a House is morally uniform, politically static, genetically homogeneous, or reducible to its founding pressure.

Each organization should eventually have:
- stable identity and name;
- organization type (House, government, institution, military body, commercial network, civic body, etc.);
- concise identity and historical rationale;
- founding/active/dissolution periods at honest precision;
- parent, constituent, predecessor, successor, alliance, rivalry, and institutional relationships where supported;
- sources and attributed claims;
- review/canon status.

## 4. Historical architecture to build

Create a root timeline for the civilization's shared history. Add child timelines only when there is a genuine parallel chronology, disputed account, or alternate branch—not merely to categorize topics.

The initial planning structure is:

1. **Pre-Imperial Civilizations** — civilizations, political systems, trade, knowledge, culture, and institutions before Imperial consolidation.
2. **The Founding Conditions** — pressures and events that made a coalition or empire possible.
3. **The Imperial Compact** — the actual founding arrangements, powers granted, constraints, representation, and unresolved compromises.
4. **The Distance Crisis** — source-defined crisis and the institutional centralization associated with it.
5. **The Thirteen Houses** — emergence and changing roles of the Houses, including their relationships before and after consolidation.
6. **The Empress and Imperial Consolidation** — her documented/source-defined position, choices, alliances, strategy, limits, and consequences.
7. **Imperial Development** — successive reforms, conflicts, discoveries, economic/cultural changes, successions, and institutional transformations.
8. **The Present Conflict** — the inherited conditions that make the game's current situation possible.

These are planning points, not asserted chronological facts. Their order and labels must be revised if source evidence establishes a different sequence. Do not invent dates to fill gaps.

## 5. Event authoring contract

A major event should capture, where known:
- stable identity, title, summary, and approximate time period;
- timeline and placement confidence;
- causes and consequences as explicit event links;
- participating organizations and individual characters as distinct entity types;
- locations and affected populations;
- sources and attributed historical claims;
- canon/review status;
- what is disputed, unknown, remembered, concealed, or propagandized.

Do not turn a thematic relationship into a causal link without evidence. Do not imply that a later faction's present-day disposition was inevitable at its founding.

## 6. Questions that must remain open until raWWar answers

- What exactly was the pre-Imperial political map and how many independent civilizations existed?
- What did the Imperial Compact grant, prohibit, and leave ambiguous?
- What happened during the Distance Crisis, and what evidence supports competing accounts?
- Did all thirteen Houses exist before the Empire, or did some form during its consolidation?
- What were the Houses' original institutional forms, territories, constituencies, and sources of legitimacy?
- What is the Empress's actual constitutional position, and how does it differ from practical power?
- What powers remain with planetary, local, and House governments?
- Which historical accounts are public, classified, suppressed, religious, regional, or contested?
- What is the causal chain from the founding settlement to the present war?

## 7. First proof of StoryBuilder's value

The first usable workspace should let an editor:
1. inspect the source record and its provenance;
2. inspect thirteen distinct organization records;
3. navigate a shared historical timeline and its planning points;
4. promote selected planning points into proposed events without accepting them;
5. link event causes and consequences;
6. attach source-backed historical claims;
7. compare conflicting claims without overwriting either;
8. inspect what a particular character or observer knows;
9. run continuity validation and review unresolved gaps;
10. save, close, and reopen the same workspace without losing identity or provenance.

A markdown dossier alone does not satisfy this proof. The authoring data must live in StoryBuilder's domain model and persistence format.

## 8. Change discipline

This session is isolated on `feature/organization-history-authoring`. It must not overwrite the active Vesper Reach authored-workspace work. No merge, release, NuGet publication, deployment, or branch deletion is authorized by this work.
