# StoryBuilderLivingMapRendererTests

![The Singularity Workshop moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

## Purpose

This test calls the same renderer used by the Living Map page. It verifies that a newly created character, role, and attributed trait change the SVG output. This is evidence of a data-derived manifestation, not a static illustration.

## Verification

The test project references the WebDemo renderer and the pinned GUI.SVG source project. CI initializes the GUI source submodule before building and testing.
