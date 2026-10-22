# CharacterProfile UML

## Class diagram

```mermaid
classDiagram
class CharacterProfile {
  +Guid Id
  +string Name
  +string Role
  +CharacterNarrativeRole NarrativeRole
  +string Description
  +CanonStatus Status
  +List~CharacterTrait~ Traits
  +List~CharacterRelationship~ Relationships
  +Rename(name) CharacterProfile
  +AddTrait(trait) CharacterProfile
}
class CharacterNarrativeRole {
  <<enumeration>>
  Undecided
  Hero
  Neutral
  Antagonist
  Other
}
class CharacterTrait {
  +string Term
  +double Intensity
  +string Attribution
  +CanonStatus Status
}
class CharacterRelationship {
  +Guid TargetCharacterId
  +string Label
  +CanonStatus Status
}
CharacterProfile --> CharacterNarrativeRole
CharacterProfile o-- CharacterTrait
CharacterProfile o-- CharacterRelationship
```

## Use case

A user opens Casting, starts an optional template, and creates a draft without completing a biography. Traits are stored with surface wording and attribution; later edits preserve the character's identity.
