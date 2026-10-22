using System.Security.Cryptography;
using System.Text;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class PublishedBookEditionTests
{
    [Fact]
    public void PublishingFreezesMarkdownAndCreatesCorrectionLineage()
    {
        var session = new StoryBuilderSession();
        var manuscript = session.CreateManuscript("The North Gate", author: "T. Best");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Arrival");
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Arrival", "Mara opens the gate.");

        var first = session.PublishManuscript(manuscript.Id);
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Arrival", "Mara refuses to open the gate.");
        var correction = session.PublishManuscript(manuscript.Id, "Corrected the protagonist's decision.");

        Assert.NotEqual(first.Id, correction.Id);
        Assert.Null(first.PreviousEditionId);
        Assert.Equal(first.Id, correction.PreviousEditionId);
        Assert.Equal("Corrected the protagonist's decision.", correction.CorrectionNotes);
        Assert.Contains("Mara opens the gate.", first.Markdown);
        Assert.DoesNotContain("refuses", first.Markdown);
        Assert.Contains("Mara refuses to open the gate.", correction.Markdown);
        Assert.Equal(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(first.Markdown))), first.ContentSha256);
        Assert.Equal(2, session.Workspace.PublishedEditions.Count);
        var frozenSections = Assert.IsAssignableFrom<IList<Guid>>(first.SectionIds);
        Assert.True(frozenSections.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => frozenSections.Add(Guid.NewGuid()));
        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
    }

    [Fact]
    public void WorkspaceValidationRejectsTamperedPublishedContent()
    {
        var session = new StoryBuilderSession();
        var manuscript = session.CreateManuscript("Integrity");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter);
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Chapter 1", "Original text.");
        var published = session.PublishManuscript(manuscript.Id);
        session.Workspace.AddPublishedEdition(published with { Id = Guid.NewGuid(), Markdown = published.Markdown + "tampered" });

        var issue = Assert.Single(StoryWorkspaceValidator.Validate(session.Workspace), item => item.Relationship == "content hash");
        Assert.Contains("SHA-256", issue.Message);
    }

    [Fact]
    public void PublishedEditionsAndCorrectionLineageSurviveBinaryRoundTrip()
    {
        var session = new StoryBuilderSession("Publication persistence");
        var manuscript = session.CreateManuscript("The Cartographer");
        var section = session.AddManuscriptSection(manuscript.Id, ManuscriptSectionKind.Chapter, "Chapter One");
        var scene = session.CreateScene("Opening", "The archive");
        session.LinkSceneToManuscriptSection(manuscript.Id, section.Sections[0].Id, scene.Id);
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Chapter One", "The map was wrong.");
        var first = session.PublishManuscript(manuscript.Id);
        session.UpdateManuscriptSection(manuscript.Id, section.Sections[0].Id, "Chapter One", "The map was deliberately wrong.");
        var correction = session.PublishManuscript(manuscript.Id, "Clarified the reveal.");

        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        var editions = restored.Workspace.PublishedEditions;
        Assert.Equal(2, editions.Count);
        Assert.Equal(first.Id, editions[0].Id);
        Assert.Equal(first.Markdown, editions[0].Markdown);
        Assert.Equal(first.ContentSha256, editions[0].ContentSha256);
        Assert.Contains(scene.Id, editions[0].SceneIds);
        Assert.Equal(correction.Id, editions[1].Id);
        Assert.Equal(first.Id, editions[1].PreviousEditionId);
        Assert.Equal("Clarified the reveal.", editions[1].CorrectionNotes);
        Assert.Empty(StoryWorkspaceValidator.Validate(restored.Workspace));
    }
}
