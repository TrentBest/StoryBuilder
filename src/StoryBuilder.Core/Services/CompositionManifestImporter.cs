using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// Imports the source-driven composition manifest into the canonical StoryWorkspace model.
/// The manifest remains the authoritative source; this importer creates editable workspace records.
/// </summary>
public static class CompositionManifestImporter
{
    public static StoryBuilderSession Import(string manifestJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestJson);

        using var document = JsonDocument.Parse(manifestJson);
        var root = document.RootElement;
        var title = RequiredString(root, "title");
        var session = new StoryBuilderSession(title);
        var sourceElement = root.GetProperty("source");
        var sourcePath = RequiredString(sourceElement, "path");
        var sourceSha = RequiredString(sourceElement, "blobSha");
        var source = new SourceRecord
        {
            Id = StableId($"source:{sourcePath}:{sourceSha}"),
            Title = "Lenny source manuscript transcription",
            SourceType = "Author manuscript transcription",
            LocationOrUri = sourcePath,
            Perspective = "Authoritative transcription; verify against the original PDF when available.",
            ReliabilityNote = $"Git blob SHA-1: {sourceSha}"
        };
        session.Workspace.AddSource(source);

        var manuscript = session.CreateManuscript(title);
        AddSection(session, manuscript.Id, ManuscriptSectionKind.FrontMatter,
            "Dedication", root.GetProperty("dedication").GetProperty("text").GetString() ?? string.Empty);

        var sections = new Dictionary<string, Guid>(StringComparer.Ordinal);
        var pageNumber = 0;
        foreach (var page in root.GetProperty("storyPages").EnumerateArray())
        {
            pageNumber++;
            var declaredNumber = page.GetProperty("pageNumber").GetInt32();
            if (declaredNumber != pageNumber)
                throw new InvalidDataException($"Expected story page {pageNumber}, found {declaredNumber}.");

            var stablePageId = RequiredString(page, "id");
            var body = page.GetProperty("text").GetString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(body))
                throw new InvalidDataException($"Story page {pageNumber} has no text.");

            var section = AddSection(session, manuscript.Id, ManuscriptSectionKind.Scene,
                $"Page {pageNumber:00}", body);
            sections.Add(stablePageId, section.Id);
        }

        if (pageNumber != 33)
            throw new InvalidDataException($"The Lenny composition manifest must contain 33 story pages; found {pageNumber}.");

        AddSection(session, manuscript.Id, ManuscriptSectionKind.BackMatter,
            "Closing page", root.GetProperty("closingPage").GetProperty("text").GetString() ?? string.Empty);

        var cast = new[]
        {
            ("Lenny", "Protagonist", "The boy at the center of the story."),
            ("Dad", "Parent", "Lenny's father."),
            ("Dolphin", "Animal ally", "The dolphin who first approaches Lenny."),
            ("Dolphin pod", "Animal group", "The group of dolphins who guide Lenny."),
            ("Beachgoers", "Group", "People present on the beach.")
        };
        var characters = new Dictionary<string, CharacterProfile>(StringComparer.Ordinal);
        foreach (var item in cast)
        {
            var character = session.CreateCharacter(item.Item1);
            character = session.UpdateCharacter(character with
            {
                Role = item.Item2,
                Description = item.Item3,
                Status = CanonStatus.Accepted,
                Notes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Visual design"] = "Unspecified by the manuscript; any generated appearance remains provisional until author approval."
                }
            });
            characters.Add(item.Item1, character);
            session.CreateVisualAsset(
                $"{item.Item1} — character design",
                VisualAssetKind.CharacterDesign,
                characterId: character.Id,
                sourceIds: [source.Id]);
        }

        var beach = session.CreatePlace("Beach", "The beach where Lenny plays and helps clean up.", "Coastal setting");
        var shoreline = session.CreatePlace("Shoreline", "The water's edge where Lenny enters and returns from the sea.", "Coastal setting", beach.Id);
        var openWater = session.CreatePlace("Open water", "The ocean setting traversed with the dolphins.", "Ocean setting");
        var oceanFloor = session.CreatePlace("Ocean floor", "An underwater environment visited during the adventure.", "Underwater setting");
        var coralReef = session.CreatePlace("Coral reef", "A reef environment encountered underwater.", "Underwater setting", oceanFloor.Id);

        foreach (var place in new[] { beach, shoreline, openWater, oceanFloor, coralReef })
        {
            session.CreateVisualAsset(
                $"{place.Name} — environment design",
                VisualAssetKind.EnvironmentDesign,
                sourceIds: [source.Id]);
        }

        foreach (var entry in sections)
        {
            session.CreateVisualAsset(
                $"Illustration — {entry.Key}",
                VisualAssetKind.PageIllustration,
                manuscriptSectionId: entry.Value,
                sourceIds: [source.Id]);
        }

        return session;
    }

    private static ManuscriptSection AddSection(
        StoryBuilderSession session,
        Guid manuscriptId,
        ManuscriptSectionKind kind,
        string title,
        string body)
    {
        var manuscript = session.AddManuscriptSection(manuscriptId, kind, title);
        var section = manuscript.Sections[^1];
        session.UpdateManuscriptSection(manuscriptId, section.Id, title, body);
        return session.Workspace.Manuscripts.Single(item => item.Id == manuscriptId)
            .Sections.Single(item => item.Id == section.Id);
    }

    private static string RequiredString(JsonElement element, string propertyName)
    {
        var value = element.GetProperty(propertyName).GetString();
        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidDataException($"Manifest property '{propertyName}' is required.")
            : value;
    }

    private static Guid StableId(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return new Guid(hash.AsSpan(0, 16));
    }
}
