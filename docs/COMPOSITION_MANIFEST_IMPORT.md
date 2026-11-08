# Composition Manifest Import

`CompositionManifestImporter` turns the source-driven JSON manifest into the existing `StoryWorkspace` model. It is deliberately an importer, not a second project format.

## Contract

- The manifest's source path and Git blob SHA become a `SourceRecord`.
- The dedication, 33 numbered story pages, and closing page become 35 ordered `ManuscriptSection` records.
- Page numbers must be contiguous and ordered from 1 through 33. Missing, empty, or out-of-order pages fail closed.
- Cast records represent only identities and story roles supported by the manuscript. Appearance remains unspecified; generated designs are explicitly provisional.
- Beach and underwater locations become `PlaceRecord` records.
- Character, environment, and page-illustration targets become `VisualAssetRecord` records linked to their character, section, and source where applicable.
- The importer does not create image bytes, approve artwork, publish an edition, or deploy a reader.

## Usage

```csharp
var session = CompositionManifestImporter.Import(File.ReadAllText(manifestPath));
var workspace = session.Workspace;
```

The imported workspace can then be saved through the existing persistence adapter. The manifest remains the canonical source for transcription and page order; re-importing it creates a fresh workspace rather than mutating an existing one.

## Lenny pilot

The Lenny manifest in `TheSingularityWorkshop.Experiences/experiences/Lenny/CompositionManifest.json` is the first consumer. It is kept with the Experience's source material rather than copied into StoryBuilder. The importer expects exactly 33 story pages for this pilot so truncation or a malformed source cannot quietly produce a partial book.
