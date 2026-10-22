# Book Preview

**User moniker:** See the actual product

**Purpose:** Renders the current ManuscriptEdition records as a live book preview. It is driven by persisted manuscript metadata, section order, and authored text; it is not a static design mockup.

## UML class card

- Route: /book
- Domain source: StoryBuilderSession and StoryWorkspace.Manuscripts
- Presentation: ordered manuscript sections and actual text
- Actions: create manuscript, add section, edit and save section text

## Use cases

- Create a working book and add chapters or passages.
- Write real text and inspect the product layout as it changes.
- Navigate between manuscript structure and the rendered preview.
- Later, follow stable links from a manuscript section to the events and scenes it portrays.
