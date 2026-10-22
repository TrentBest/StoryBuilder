using StoryBuilder.Core.Models;

namespace StoryBuilder.Core.Services;

/// <summary>
/// In-memory domain-wide vocabulary index. It preserves surface wording and attribution,
/// and treats synonyms as an explicit, editable author-maintained mapping.
/// </summary>
public sealed class VocabularyIndex
{
    private readonly List<WordOccurrence> _occurrences = [];
    private readonly Dictionary<string, VocabularyConcept> _concepts = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();

    public IReadOnlyList<WordOccurrence> Occurrences
    {
        get { lock (_gate) return _occurrences.ToArray(); }
    }

    public VocabularyConcept AddSynonym(string canonicalTerm, string synonym, VocabularyCategory category = VocabularyCategory.Adjective)
    {
        ValidateTerm(canonicalTerm, nameof(canonicalTerm));
        ValidateTerm(synonym, nameof(synonym));
        lock (_gate)
        {
            var canonical = Normalize(canonicalTerm);
            var alias = Normalize(synonym);
            if (!_concepts.TryGetValue(canonical, out var concept))
                concept = new VocabularyConcept { CanonicalTerm = canonicalTerm.Trim(), Category = category };

            var synonyms = concept.Synonyms
                .Where(s => !string.Equals(Normalize(s), alias, StringComparison.OrdinalIgnoreCase))
                .Append(synonym.Trim())
                .Where(s => !string.Equals(Normalize(s), canonical, StringComparison.OrdinalIgnoreCase))
                .ToList();

            concept = concept with { Synonyms = synonyms, Category = category };
            _concepts[canonical] = concept;
            _concepts[alias] = concept;
            return concept;
        }
    }

    public WordOccurrence Record(
        string surfaceForm,
        VocabularyCategory category,
        Guid targetId,
        string targetLabel,
        string field,
        string? context = null,
        string attribution = "Author",
        string? sourceId = null,
        CanonStatus status = CanonStatus.Draft)
    {
        ValidateTerm(surfaceForm, nameof(surfaceForm));
        var normalized = Normalize(surfaceForm);
        lock (_gate)
        {
            var canonical = _concepts.TryGetValue(normalized, out var concept)
                ? concept.CanonicalTerm
                : surfaceForm.Trim();
            var occurrence = new WordOccurrence
            {
                SurfaceForm = surfaceForm.Trim(),
                CanonicalTerm = canonical,
                Category = category,
                TargetId = targetId,
                TargetLabel = targetLabel?.Trim() ?? string.Empty,
                Field = field?.Trim() ?? string.Empty,
                Context = context,
                Attribution = string.IsNullOrWhiteSpace(attribution) ? "Author" : attribution.Trim(),
                SourceId = sourceId,
                Status = status
            };
            _occurrences.Add(occurrence);
            return occurrence;
        }
    }

    public IReadOnlyList<VocabularyUsage> Search(string query)
    {
        ValidateTerm(query, nameof(query));
        var normalized = Normalize(query);
        lock (_gate)
        {
            var canonical = _concepts.TryGetValue(normalized, out var concept)
                ? concept.CanonicalTerm
                : query.Trim();

            var matches = _occurrences
                .Where(o => string.Equals(Normalize(o.CanonicalTerm), Normalize(canonical), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Normalize(o.SurfaceForm), normalized, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count == 0)
                return Array.Empty<VocabularyUsage>();

            return
            [
                new VocabularyUsage
                {
                    CanonicalTerm = canonical,
                    OccurrenceCount = matches.Count,
                    DistinctTargetCount = matches.Select(o => o.TargetId).Distinct().Count(),
                    Occurrences = matches
                }
            ];
        }
    }

    public IReadOnlyList<VocabularyUsage> GetMostUsed(int take = 20)
    {
        if (take < 1)
            throw new ArgumentOutOfRangeException(nameof(take), "Take must be at least one.");

        lock (_gate)
        {
            return _occurrences
                .GroupBy(o => Normalize(o.CanonicalTerm), StringComparer.OrdinalIgnoreCase)
                .Select(group => new VocabularyUsage
                {
                    CanonicalTerm = group.First().CanonicalTerm,
                    OccurrenceCount = group.Count(),
                    DistinctTargetCount = group.Select(o => o.TargetId).Distinct().Count(),
                    Occurrences = group.ToArray()
                })
                .OrderByDescending(u => u.OccurrenceCount)
                .ThenBy(u => u.CanonicalTerm, StringComparer.OrdinalIgnoreCase)
                .Take(take)
                .ToArray();
        }
    }

    private static string Normalize(string value) => string.Join(' ', value.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static void ValidateTerm(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A non-empty term is required.", parameterName);
    }
}
