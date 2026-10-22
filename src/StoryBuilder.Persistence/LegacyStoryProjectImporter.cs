using System.Globalization;
using System.Text.Json;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Persistence;

/// <summary>
/// Imports the first-generation JSON project schema into the current workspace model.
/// The exact source JSON is retained on the workspace so fields not yet represented by
/// the richer model (including passage anchors and observer IDs) are never silently lost.
/// </summary>
public static class LegacyStoryProjectImporter
{
    private static readonly JsonDocumentOptions DocumentOptions = new() { CommentHandling = JsonCommentHandling.Skip };

    public static StoryWorkspace Import(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        using var document = JsonDocument.Parse(json, DocumentOptions);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("The legacy StoryBuilder project must be a JSON object.");

        var schemaVersion = GetInt(root, "schemaVersion", 1);
        if (schemaVersion != 1)
            throw new InvalidDataException($"Unsupported legacy StoryBuilder schema {schemaVersion}; expected 1.");

        var projectId = GetGuid(root, "projectId") ?? Guid.NewGuid();
        var title = GetString(root, "title") ?? "Untitled Story";
        var premiseText = GetString(root, "premise") ?? string.Empty;
        var manuscriptText = GetString(root, "manuscript") ?? string.Empty;
        var workspace = new StoryWorkspace
        {
            Id = projectId,
            Name = string.IsNullOrWhiteSpace(title) ? "Untitled Story" : title,
            LegacyProjectSourceJson = json
        };

        workspace.AddPremise(new StoryPremise
        {
            Id = StableDerivedId(projectId, "premise"),
            Title = string.IsNullOrWhiteSpace(title) ? "Untitled Story" : title,
            Vision = premiseText,
            Status = CanonStatus.Draft,
            CreatedAtUtc = GetDateTimeOffset(root, "createdUtc") ?? DateTimeOffset.UtcNow,
            UpdatedAtUtc = GetDateTimeOffset(root, "updatedUtc") ?? DateTimeOffset.UtcNow
        });

        var timeline = new TimelineDefinition
        {
            Id = StableDerivedId(projectId, "timeline"),
            Name = "Imported timeline",
            Description = "Timeline imported from a legacy StoryBuilder project."
        };
        workspace.AddTimeline(timeline);

        var passages = GetArray(root, "passages");
        var passageById = new Dictionary<Guid, JsonElement>();
        foreach (var passage in passages)
        {
            if (GetGuid(passage, "id") is Guid id)
                passageById.TryAdd(id, passage);
        }

        var manuscript = new ManuscriptEdition
        {
            Id = StableDerivedId(projectId, "manuscript"),
            Title = title,
            Author = string.Empty,
            Status = CanonStatus.Draft,
            Sections = string.IsNullOrEmpty(manuscriptText)
                ? []
                : [new ManuscriptSection
                {
                    Id = StableDerivedId(projectId, "manuscript-body"),
                    Sequence = 1,
                    Kind = ManuscriptSectionKind.Chapter,
                    Title = "Imported manuscript",
                    Body = manuscriptText
                }],
            CreatedAtUtc = GetDateTimeOffset(root, "createdUtc") ?? DateTimeOffset.UtcNow,
            UpdatedAtUtc = GetDateTimeOffset(root, "updatedUtc") ?? DateTimeOffset.UtcNow
        };
        workspace.AddManuscript(manuscript);

        foreach (var legacyEvent in GetArray(root, "timeline"))
        {
            var eventId = GetGuid(legacyEvent, "id") ?? Guid.NewGuid();
            var occurredAt = GetDateTimeOffset(legacyEvent, "occurredAt");
            var passageId = GetGuid(legacyEvent, "passageId");
            var passageText = passageId is Guid pid && passageById.TryGetValue(pid, out var passage)
                ? GetString(passage, "text")
                : null;
            var summary = GetString(legacyEvent, "description") ?? string.Empty;
            if (!string.IsNullOrEmpty(passageText))
                summary = string.IsNullOrEmpty(summary) ? passageText : summary + Environment.NewLine + passageText;

            workspace.AddEvent(new StoryEvent
            {
                Id = eventId,
                Title = GetString(legacyEvent, "title") ?? "Untitled event",
                Summary = summary,
                InWorldDateOrPeriod = occurredAt?.ToString("O", CultureInfo.InvariantCulture),
                TimelineId = timeline.Id,
                PlacementKind = occurredAt.HasValue ? TimelinePlacementKind.Approximate : TimelinePlacementKind.Unresolved,
                Status = CanonStatus.Proposed
            });
        }

        return workspace;
    }

    private static Guid StableDerivedId(Guid sourceId, string purpose)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(sourceId.ToString("N") + ":" + purpose));
        return new Guid(bytes.AsSpan(0, 16));
    }

    private static string? GetString(JsonElement element, string name) =>
        TryGet(element, name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static Guid? GetGuid(JsonElement element, string name) =>
        Guid.TryParse(GetString(element, name), out var value) ? value : null;

    private static int GetInt(JsonElement element, string name, int fallback) =>
        TryGet(element, name, out var value) && value.TryGetInt32(out var result) ? result : fallback;

    private static DateTimeOffset? GetDateTimeOffset(JsonElement element, string name) =>
        DateTimeOffset.TryParse(GetString(element, name), CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var value) ? value : null;

    private static List<JsonElement> GetArray(JsonElement element, string name) =>
        TryGet(element, name, out var value) && value.ValueKind == JsonValueKind.Array
            ? value.EnumerateArray().ToList()
            : [];

    private static bool TryGet(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
