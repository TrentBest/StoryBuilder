# StoryContinuityAnalyzerTests

**Purpose:** Verify that continuity analysis detects graph conflicts without mutating the author's records.

## Covered behavior

- Detects causal cycles.
- Detects disagreement between cause and consequence links.
- Detects cyclic timeline-parent relationships.
- Accepts a consistent, acyclic causal history without findings.
