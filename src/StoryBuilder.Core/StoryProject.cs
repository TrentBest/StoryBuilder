namespace TheSingularityWorkshop.StoryBuilder;

public sealed class StoryProject
{
    public const int CurrentSchemaVersion = 1;
    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    public Guid ProjectId { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = "Untitled Story";
    public string Premise { get; set; } = string.Empty;
    public string Manuscript { get; set; } = string.Empty;
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<PassageAnchor> Passages { get; init; } = [];
    public List<TimelineEvent> Timeline { get; init; } = [];

    public static StoryProject Create(string title = "Untitled Story") =>
        new() { Title = string.IsNullOrWhiteSpace(title) ? "Untitled Story" : title.Trim() };

    public TimelineEvent AddEvent(string title, string description = "",
        string? passageText = null, string? observerId = null, DateTimeOffset? occurredAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("An event title is required.", nameof(title));

        Guid? passageId = null;
        if (!string.IsNullOrWhiteSpace(passageText))
        {
            var needle = passageText.Trim();
            if (!Manuscript.Contains(needle, StringComparison.Ordinal))
                throw new ArgumentException("The linked passage must appear in the manuscript text.", nameof(passageText));

            var passage = Passages.FirstOrDefault(p => p.Text == needle);
            if (passage is null)
            {
                passage = new PassageAnchor { Id = Guid.NewGuid(), Text = needle };
                Passages.Add(passage);
            }
            passageId = passage.Id;
        }

        var item = new TimelineEvent
        {
            Id = Guid.NewGuid(), Title = title.Trim(), Description = description.Trim(),
            OccurredAt = occurredAt, PassageId = passageId,
            ObserverId = string.IsNullOrWhiteSpace(observerId) ? null : observerId.Trim()
        };
        Timeline.Add(item);
        UpdatedUtc = DateTimeOffset.UtcNow;
        return item;
    }

    public IReadOnlyList<TimelineEvent> GetChronology() =>
        Timeline.OrderBy(item => item.OccurredAt ?? DateTimeOffset.MaxValue)
            .ThenBy(item => item.Title, StringComparer.Ordinal).ToArray();
}

public sealed class TimelineEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset? OccurredAt { get; set; }
    public Guid? PassageId { get; set; }
    public string? ObserverId { get; set; }
}

public sealed class PassageAnchor
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Text { get; set; } = string.Empty;
}
