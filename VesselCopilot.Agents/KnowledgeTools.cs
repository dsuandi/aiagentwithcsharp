using System.ComponentModel;

public static class KnowledgeTools
{
    private static readonly Dictionary<string, (string Title, string Text)> Library = new()
    {
        ["heavy weather"] = ("HW-07-heavy-weather", "HW-07 rev 4: Reduce speed, secure deck cargo, brief the crew, log position and weather hourly."),
        ["temperature"] = ("MX-02-engine-temperature", "MX-02 rev 2: Above 90 C or a swing of more than 8 C: reduce load, check pump and thermostatic valve, inform the chief engineer, raise a maintenance draft.")
    };

    [Description("Searches the synthetic approved procedure library, returning source code and revision. Query must be at most 200 characters.")]
    public static string SearchKnowledgeBase(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length > 200)
            return "Error: query must be 1 to 200 characters.";
        List<VesselChunk> hits = Library.Where(entry => query.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
            .Select(entry => new VesselChunk { Title = entry.Value.Title, Content = entry.Value.Text }).ToList();
        return hits.Count > 0 ? GroundedAnswer.FormatSources(hits) : "No approved procedure found for that query.";
    }
}