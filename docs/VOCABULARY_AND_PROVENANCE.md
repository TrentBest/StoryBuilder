# Vocabulary and Provenance

## One vocabulary across the story domain

Authors need to ask questions such as:
- Where does “brave” appear?
- Is “fearless” being used as a synonym, or does this speaker mean something different?
- Who called this character “dangerous”?
- Does the same word describe a place, faction, and person?
- Which terms have become common in this campaign?

VocabularyIndex currently supports explicit synonym mappings and records each occurrence with its original surface form, canonical term, target, field, context, attribution, source reference, and status.

## Preserve the difference between word and concept

A synonym map is a navigational aid, not a license to rewrite prose. The user can search one concept and see that different people used different words. Attribution matters: “Mara is reckless” may be her own judgment, a rival's accusation, or the narrator's voice. Those are not equivalent facts.

## Provenance and canon

The workspace includes source records, historical claims, proposed events, participant actions, and continuity issues. These fields support the distinction between:
- a source's statement;
- the story fact the author accepts;
- an in-world belief or rumor;
- a participant's proposed action;
- an accepted campaign consequence.

Confidence is optional. A low-confidence memory or disputed account can still be useful without being treated as canon.

## Current limitations

The vocabulary index is in-memory, and synonyms are added explicitly. Automatic adjective extraction, multilingual stemming, semantic embeddings, and incremental persistent indexing are not implemented yet.
