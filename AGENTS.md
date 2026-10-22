# Repository Instructions for AI Agents

## Branch policy

The permanent branch model is exactly:
- master — stable, reviewed, release-ready history.
- development — active integration and verification.

Work directly on development. Do not create feature or handoff branches. Preserve useful work, record reconciliation evidence, and never delete branches on the owner's behalf. The owner performs any manual branch deletion.

## Architecture

- StoryBuilder is a generic Experience/Tool authoring system; its science-fiction universe is a demonstration, not a required dependency.
- Keep the domain core host-neutral and dependency-light.
- FSM_API may express workflow behavior where a finite-state model is appropriate; FSM_COS is the composition boundary, not the owner of StoryBuilder's domain.
- Do not introduce dependencies just because a neighboring package exists. Verify contracts before integrating them.
- Keep implementation records separate from fictional canon.
- Preserve attribution, provenance, source links, draft/proposed/accepted status, and original wording.
- GUI.SVG visualizations must reflect real model data; no static mockups presented as live behavior.
- Do not publish packages, create releases, deploy, or enable publishing workflows without explicit owner authorization.

## Documentation and verification

Follow the shared [FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/DOCUMENTATION_STANDARD.md). Every maintained C# source file should have a companion Markdown page; maintained types should have source-grounded UML class diagrams and use cases. Keep README claims evidence-based and distinguish implemented behavior from intended integration.

Run tests/builds when the environment permits. If execution is unavailable, say so explicitly and rely on CI evidence before calling a workflow verified.
