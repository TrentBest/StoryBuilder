using System.Text;
using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>Produces a deterministic Markdown book product from canonical manuscript records.</summary>
public sealed class BookProductRenderer
{
    public RenderedBookArtifact Render(StoryWorkspace workspace, Guid manuscriptId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var manuscript = workspace.Manuscripts.FirstOrDefault(item => item.Id == manuscriptId)
            ?? throw new KeyNotFoundException($"Manuscript '{manuscriptId}' does not exist in this workspace.");

        var title = NonEmpty(manuscript.Title, "Untitled book");
        var subtitle = manuscript.Subtitle?.Trim() ?? string.Empty;
        var author = manuscript.Author?.Trim() ?? string.Empty;
        var sections = manuscript.Sections.OrderBy(section => section.Sequence).ToList();
        var output = new StringBuilder();
        output.Append("# ").Append(title).Append('\n');
        if (!string.IsNullOrWhiteSpace(subtitle))
            output.Append('\n').Append(subtitle).Append('\n');
        if (!string.IsNullOrWhiteSpace(author))
            output.Append("\n**Author:** ").Append(author).Append('\n');

        foreach (var section in sections)
        {
            output.Append('\n');
            output.Append(Heading(section.Kind)).Append(' ').Append(NonEmpty(section.Title, $"Section {section.Sequence}")).Append('\n');
            var body = NormalizeLineEndings(section.Body).Trim();
            if (body.Length > 0)
                output.Append('\n').Append(body).Append('\n');
        }

        var referencedEventIds = sections
            .SelectMany(section => section.EventIds)
            .Distinct()
            .ToArray();
        return new RenderedBookArtifact(
            manuscript.Id,
            title,
            subtitle,
            author,
            output.ToString().TrimEnd('\n') + "\n",
            sections.Count,
            referencedEventIds);
    }

    /// <summary>Writes the deterministic Markdown artifact to a caller-owned writable stream without closing it.</summary>
    public RenderedBookArtifact WriteMarkdown(StoryWorkspace workspace, Guid manuscriptId, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite)
            throw new ArgumentException("The destination stream is not writable.", nameof(destination));

        var artifact = Render(workspace, manuscriptId);
        var bytes = Encoding.UTF8.GetBytes(artifact.Markdown);
        destination.Write(bytes);
        return artifact;
    }

    private static string Heading(ManuscriptSectionKind kind) => kind switch
    {
        ManuscriptSectionKind.FrontMatter => "##",
        ManuscriptSectionKind.Chapter => "##",
        ManuscriptSectionKind.Scene => "###",
        ManuscriptSectionKind.Passage => "####",
        ManuscriptSectionKind.BackMatter => "##",
        _ => "##"
    };

    private static string NonEmpty(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static string NormalizeLineEndings(string? value) =>
        (value ?? string.Empty).Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
}
