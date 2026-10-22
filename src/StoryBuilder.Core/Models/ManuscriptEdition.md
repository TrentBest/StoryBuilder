# ManuscriptEdition

**User moniker:** Working book / manuscript product

**Purpose:** Holds the editable manuscript identity and ordered sections that form a real product preview. It remains separate from world history; a section may link to scenes and events without becoming the event itself.

## UML class card

- Identity: Id
- Product metadata: Title, Subtitle, Author, Status
- Composition: ordered ManuscriptSection records
- Lifecycle: CreatedAtUtc, UpdatedAtUtc
- Operations: AddSection, ReplaceSection

## Use cases

- Create a book before its contents are known.
- Add chapters, scenes, passages, front matter, and back matter in reading order.
- Revise actual text and see the rendered product update from saved records.
- Link manuscript sections to scenes and events as those relationships are implemented.
- Preserve working draft identity separately from future immutable publication editions.
