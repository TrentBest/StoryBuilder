# OrganizationAuthoringTests

## Purpose
Verifies first-class organization authoring, distinct organization participation in events, validation, vocabulary refresh, and versioned workspace persistence.

## Use cases
- Create a political house or institution without representing it as a character.
- Associate an organization with an event independently of character participants.
- Reject dangling organization references and workspace-wide identity collisions.
- Persist and restore organization records and event organization-participant IDs.

## Verification
Run `dotnet test tests/StoryBuilder.Core.Tests/StoryBuilder.Core.Tests.csproj`.
