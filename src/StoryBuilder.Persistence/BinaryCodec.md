# BinaryCodec

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

BinaryCodec centralizes the workspace format's little-endian primitives, strict UTF-8 strings, GUIDs, dates, enum validation, collection counts, and exact reads. It rejects invalid sizes and truncated input before allocating unbounded data.

## Boundary

This helper is internal to StoryBuilder.Persistence. The semantic record layout and format version live in StoryWorkspaceBinaryDocument.
