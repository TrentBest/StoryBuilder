# RaWWar Empire Workspace Builder — UML

The Quickstart builder composes existing StoryBuilder domain records. It does not introduce a raWWar-specific type into the generic StoryBuilder core.

```mermaid
classDiagram
    class RaWWarEmpireWorkspace {
        +SourceUri string
        +Create() StoryBuilderSession
    }
    class StoryBuilderSession {
        +Workspace StoryWorkspace
        +AddSource(SourceRecord) SourceRecord
        +CreateOrganization(name, kind, description) OrganizationRecord
        +AddHistoricalClaim(HistoricalClaim) HistoricalClaim
        +CreateTimeline(name, description) TimelineDefinition
        +CreatePlanningPoint(timelineId, label, placement, intent) TimelinePlanningPoint
        +AddContinuityIssue(ContinuityIssue) ContinuityIssue
    }
    class StoryWorkspace {
        +Organizations OrganizationRecord[]
        +Sources SourceRecord[]
        +HistoricalClaims HistoricalClaim[]
        +Timelines TimelineDefinition[]
        +PlanningPoints TimelinePlanningPoint[]
        +ContinuityIssues ContinuityIssue[]
    }
    class SourceRecord
    class OrganizationRecord
    class HistoricalClaim
    class TimelineDefinition
    class TimelinePlanningPoint
    class ContinuityIssue

    RaWWarEmpireWorkspace ..> StoryBuilderSession : creates
    StoryBuilderSession --> StoryWorkspace : authors
    StoryWorkspace o-- SourceRecord
    StoryWorkspace o-- OrganizationRecord
    StoryWorkspace o-- HistoricalClaim
    StoryWorkspace o-- TimelineDefinition
    StoryWorkspace o-- TimelinePlanningPoint
    StoryWorkspace o-- ContinuityIssue
    HistoricalClaim --> SourceRecord : SourceIds
    HistoricalClaim --> OrganizationRecord : RelatedEntityIds
    TimelinePlanningPoint --> TimelineDefinition : TimelineId
```

## Lifecycle

- The source record retains the canonical raWWar document URI.
- House organizations start as Draft.
- Source-backed claims and continuity issues start as Proposed.
- Timeline anchors remain unresolved placeholders until authors supply chronology.
- No StoryEvent records are fabricated by this builder.
