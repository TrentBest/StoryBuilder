# BookProductRenderer

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

Renders a working manuscript into a deterministic Markdown digital book artifact. It orders sections by sequence, normalizes line endings across platforms, and preserves stable world-event references as metadata. `WriteMarkdown` writes the exact UTF-8 output to a caller-owned writable stream and leaves that stream open; the host chooses the file path and any atomic-replacement policy.

## UML class card

See [BookProductRenderer UML](../../../docs/UML/BookProductRendererUML.md).

## Boundary

This is an actual digital composition output, not a PDF/print layout. `WriteMarkdown` provides stream export; `PublishManuscript` creates an immutable published edition. It does not run Workshop arbitration; the same renderer can consume a restored workspace from a validated binary package.
