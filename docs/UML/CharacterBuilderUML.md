# CharacterBuilder UML

## Class diagram

```mermaid
classDiagram
class CharacterBuilder {
  +GetTemplates() IReadOnlyList~CharacterTemplate~
  +Create(name, templateName) CharacterProfile
  +FindTemplate(name) CharacterTemplate
}
class CharacterTemplate {
  +string Name
  +IReadOnlyList~string~ SuggestedPrompts
  +IReadOnlyList~string~ SuggestedTraits
}
CharacterBuilder ..> CharacterTemplate
CharacterBuilder ..> CharacterProfile
```

## Use case

Opening Casting creates a useful draft immediately. The author may select a template or ignore the prompts and start writing. Suggestions remain suggestions until the author adds them.
