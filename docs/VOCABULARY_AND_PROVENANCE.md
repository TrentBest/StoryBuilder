# Vocabulary and Provenance

## One vocabulary across the story domain

Authors need to ask questions such as:
- Where does “brave” appear?
- Is “fearless” being used as a synonym, or does this speaker mean something different?
- Who called this character “dangerous”?
- Does the same word describe a place, faction, and person?
- Which terms have become common in this campaign?

VocabularyIndex supports explicit synonym mappings and records each occurrence with its original surface form, canonical term, target, field, context, attribution, source reference, and status. StoryBuilderSession automatically indexes character names, roles, descriptions, traits, scene metadata, and captured beat text. Editing a character or scene replaces that target's prior occurrences before re-indexing the updated record.

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

The index is in-memory. Text tokenization splits on whitespace and strips most punctuation; it does not perform automatic adjective classification, multilingual stemming, semantic embeddings, or thesaurus generation. Synonyms remain explicit and editable.
