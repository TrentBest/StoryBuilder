# StoryBuilder Documentation Index

| If you want to... | Start here |
|---|---|
| Understand the purpose and boundary | [README](README.md) |
| Meet the Narrator and understand the first-session flow | [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) |
| Understand the package AnyApp consumes | [Composition Package Contract](docs/COMPOSITION_PACKAGE_CONTRACT.md) |
| Add an approximate Plot Twist to a timeline | Timeline planning point model [UML](docs/UML/TimelinePlanningPointUML.md) and the WebDemo Timelines page |
| Link key events to the actual book | WebDemo Timelines and Book Preview; manuscript event references are stored by stable event ID |
| View and author a real manuscript product | WebDemo Book Preview at /book; manuscript model [UML](docs/UML/ManuscriptEditionUML.md) |
| Understand book-first composition, provisional Plot Twist planning, observer constraints, and multi-FSM Narrator | [Timeline Authoring and Observer Model](docs/TIMELINE_AUTHORING_MODEL.md), [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) |
| Understand publication output versus physical printing | [Narrator Experience — Publication Boundary](docs/NARRATOR_EXPERIENCE.md#publication-boundary) |
| Explore the casting-room concept | [Character Casting Studio](docs/CHARACTER_CASTING_STUDIO.md) |
| Understand the timeline Process Group foundation | [Timeline Process Groups](docs/TIMELINE_PROCESS_GROUPS.md), [TimelineProcessGroup UML](docs/UML/TimelineProcessGroupUML.md), [Timeline tests](tests/StoryBuilder.Core.Tests/TimelineProcessGroupTests.cs) |
| Design timeline console, event-to-book links, and immutable editions | [Timeline Process Groups](docs/TIMELINE_PROCESS_GROUPS.md), [Timeline Authoring and Observer Model](docs/TIMELINE_AUTHORING_MODEL.md) |
| Design plural timelines, time-travel branches, gestures, and multiverse history | [Timelines, Gestures, and Multiverse History](docs/TIMELINES_GESTURES_MULTIVERSE.md) |
| Inspect character words, context, and pronoun references | [Character Casting Studio — Word and Reference Observability](docs/CHARACTER_CASTING_STUDIO.md#word-and-reference-observability) |
| Run the authoring workbench | [WebDemo guide](samples/StoryBuilder.WebDemo/README.md) |
| Inspect living GUI.SVG views | [WebDemo guide](samples/StoryBuilder.WebDemo/README.md), [WebDemo UML](docs/UML/StoryBuilderWebDemoUML.md) |
| Understand architecture and stack fit | [Architecture](docs/ARCHITECTURE.md) |
| Start a story with one idea | [Narrator Experience](docs/NARRATOR_EXPERIENCE.md) |
| Create and revise characters | [Character Casting Studio](docs/CHARACTER_CASTING_STUDIO.md), [Character Creation](docs/CHARACTER_CREATION.md) |
| Capture action scenes or poses | [Scene Capture](docs/SCENE_CAPTURE.md) |
| Search words, synonyms, and attribution | [Vocabulary and Provenance](docs/VOCABULARY_AND_PROVENANCE.md) |
| Understand world scale and history | [Story Foundation](docs/STORY_FOUNDATION.md), [Historical Scale](docs/HISTORICAL_SCALE.md) |
| Understand diegetic-tool concepts | [Diegetic StoryBuilder](docs/DIEGETIC_STORYBUILDER.md) |
| Inspect UML and use cases | [UML directory](docs/UML/) |
| Understand FSM_API lifecycle | [StoryLifecycleSession UML](docs/UML/StoryLifecycleSessionUML.md), [Lifecycle Registry UML](docs/UML/StoryLifecycleRegistryUML.md) |
| Understand the FSM_COS-composable bundle | [StoryBuilderAuthoringBundle UML](docs/UML/StoryBuilderAuthoringBundleUML.md) |
| Understand binary workspace persistence | [StoryWorkspaceBinaryDocument UML](docs/UML/StoryWorkspaceBinaryDocumentUML.md), [Persistence guide](src/StoryBuilder.Persistence/StoryWorkspaceBinaryDocument.md) |
| Understand browser-local autosave | [Persistence coordinator guide](samples/StoryBuilder.WebDemo/StoryBuilderPersistenceCoordinator.md) |
| Review implemented work and gaps | [Development Status](docs/DEVELOPMENT_STATUS.md) |
| Review branch preservation and integration | [Branch Reconciliation](docs/BRANCH_RECONCILIATION.md) |
| Build and test | [Development workflow](docs/DEVELOPMENT.md) |

The README is the front door. Source code and tests define current behavior; design documents describe intended capabilities and must not be mistaken for proof that those capabilities run.
