# StoryEventCausalityAuthoringTests

These tests verify event updates synchronize vocabulary indexing, cause/consequence links are reciprocal, removing an edge preserves both event records, self-causation is rejected, and missing event endpoints do not mutate the workspace.

Cycle detection remains advisory through `StoryContinuityAnalyzer`; causal authoring does not silently rewrite unrelated links.
