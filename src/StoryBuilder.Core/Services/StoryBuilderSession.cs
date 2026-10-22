using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Coordinates common authoring operations so edits and the vocabulary index stay synchronized.
/// This session is in-memory; the host is responsible for persistence.
/// </summary>
public sealed class StoryBuilderSession
{
    public StoryBuilderSession(string? workspaceName = null)
    {
        Workspace = new StoryWorkspace
        {
            Name = string.IsNullOrWhiteSpace(workspaceName) ? "StoryBuilder" : workspaceName.Trim()
        };
    }

    public StoryWorkspace Workspace { get; }
    public CharacterBuilder CharacterBuilder { get; } = new();
    public SceneComposer SceneComposer { get; } = new();
    public VocabularyIndex Vocabulary { get; } = new();

    /// <summary>Creates and indexes a new character draft immediately.</summary>
    public CharacterProfile CreateCharacter(string? name = null, string? templateName = null)
    {
        var character = CharacterBuilder.Create(name, templateName);
        Workspace.AddCharacter(character);
        IndexCharacter(character);
        return character;
    }

    /// <summary>Replaces a character draft and rebuilds its word occurrences after edits.</summary>
    public CharacterProfile UpdateCharacter(CharacterProfile character)
    {
        ArgumentNullException.ThrowIfNull(character);
        Workspace.ReplaceCharacter(character);
        Vocabulary.RemoveTarget(character.Id);
        IndexCharacter(character);
        return character;
    }

    /// <summary>Creates and stores an empty scene so the user can add beats incrementally.</summary>
    public SceneRecord CreateScene(string? title = null, string? location = null, SceneCaptureMode mode = SceneCaptureMode.Freeform)
    {
        var scene = SceneComposer.Create(title, location, mode);
        Workspace.AddScene(scene);
        IndexScene(scene);
        return scene;
    }

    /// <summary>Replaces a scene and refreshes its indexed words after beat edits.</summary>
    public SceneRecord UpdateScene(SceneRecord scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        Workspace.ReplaceScene(scene);
        Vocabulary.RemoveTarget(scene.Id);
        IndexScene(scene);
        return scene;
    }

    public HistoricalClaim AddHistoricalClaim(HistoricalClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        Workspace.AddClaim(claim);
        var sourceId = claim.SourceIds.FirstOrDefault();
        Vocabulary.RecordText(claim.Statement, VocabularyCategory.Other, claim.Id, "Historical claim",
            nameof(HistoricalClaim.Statement), context: claim.Statement, attribution: claim.Attribution,
            sourceId: sourceId == Guid.Empty ? null : sourceId.ToString("D"),
            status: claim.Status);
        return claim;
    }

    public StoryEvent AddEvent(StoryEvent storyEvent)
    {
        ArgumentNullException.ThrowIfNull(storyEvent);
        Workspace.AddEvent(storyEvent);
        Vocabulary.RecordText(storyEvent.Title, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
            nameof(StoryEvent.Title), context: storyEvent.Summary, status: storyEvent.Status);
        Vocabulary.RecordText(storyEvent.Summary, VocabularyCategory.Other, storyEvent.Id, storyEvent.Title,
            nameof(StoryEvent.Summary), context: storyEvent.Summary, status: storyEvent.Status);
        return storyEvent;
    }

    public CampaignRecord AddCampaign(CampaignRecord campaign)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        Workspace.AddCampaign(campaign);
        Vocabulary.RecordText(campaign.Name, VocabularyCategory.Other, campaign.Id, campaign.Name,
            nameof(CampaignRecord.Name), context: campaign.Description, status: campaign.Status);
        Vocabulary.RecordText(campaign.Description, VocabularyCategory.Other, campaign.Id, campaign.Name,
            nameof(CampaignRecord.Description), context: campaign.Description, status: campaign.Status);
        return campaign;
    }

    public ParticipantAction AddParticipantAction(ParticipantAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Workspace.AddParticipantAction(action);
        var campaignName = Workspace.Campaigns.FirstOrDefault(c => c.Id == action.CampaignId)?.Name ?? "Campaign action";
        Vocabulary.RecordText(action.Description, VocabularyCategory.Other, action.Id, campaignName,
            nameof(ParticipantAction.Description), context: action.Description, attribution: action.Attribution,
            sourceId: action.Id.ToString("D"), status: action.Status);
        return action;
    }

    private void IndexCharacter(CharacterProfile character)
    {
        Vocabulary.RecordText(character.Name, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Name), context: character.Name, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Role, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Role), context: character.Role, attribution: "Author", status: character.Status);
        Vocabulary.RecordText(character.Description, VocabularyCategory.Other, character.Id, character.Name,
            nameof(CharacterProfile.Description), context: character.Description, attribution: "Author",
            status: character.Status);

        foreach (var trait in character.Traits)
        {
            Vocabulary.RecordText(trait.Term, VocabularyCategory.Adjective, character.Id, character.Name,
                nameof(CharacterProfile.Traits), context: trait.Notes ?? trait.Term, attribution: trait.Attribution,
                status: trait.Status);
        }
    }

    private void IndexScene(SceneRecord scene)
    {
        Vocabulary.RecordText(scene.Title, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Title), context: scene.Purpose, status: CanonStatus.Draft);
        Vocabulary.RecordText(scene.Location, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Location), context: scene.Location, status: CanonStatus.Draft);
        Vocabulary.RecordText(scene.Purpose, VocabularyCategory.Other, scene.Id, scene.Title,
            nameof(SceneRecord.Purpose), context: scene.Purpose, status: CanonStatus.Draft);

        foreach (var beat in scene.Beats.OrderBy(b => b.Sequence))
        {
            IndexBeatText(scene, beat, beat.Action, nameof(SceneBeat.Action), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.Reaction, nameof(SceneBeat.Reaction), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.Dialogue, nameof(SceneBeat.Dialogue), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.PoseOrPosition, nameof(SceneBeat.PoseOrPosition), VocabularyCategory.Other);
            IndexBeatText(scene, beat, beat.EmotionalTone, nameof(SceneBeat.EmotionalTone), VocabularyCategory.Adjective);
            IndexBeatText(scene, beat, beat.CaptureNote, nameof(SceneBeat.CaptureNote), VocabularyCategory.Other);
        }
    }

    private void IndexBeatText(SceneRecord scene, SceneBeat beat, string? text, string field, VocabularyCategory category)
    {
        Vocabulary.RecordText(text, category, scene.Id, scene.Title, field,
            context: text, attribution: beat.Attribution, sourceId: beat.Id.ToString("D"), status: beat.Status);
    }
}
