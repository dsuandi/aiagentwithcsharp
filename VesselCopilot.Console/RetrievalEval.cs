using Microsoft.Extensions.AI;

public record GroundTruth(string Question, string ExpectedTitle);

public static class RetrievalEval
{
    public static readonly GroundTruth[] Questions =
    [
        new("What wind force triggers the heavy-weather procedure?", "HW-07-heavy-weather"),
        new("How often must the watch log position during heavy weather?", "HW-07-heavy-weather"),
        new("What should the crew do when the engine runs hot?", "MX-02-engine-temperature"),
        new("Which revision of the heavy-weather procedure is current?", "HW-07-heavy-weather"),
        new("What is the crew change schedule for VL-204?", ""),
        new("What jacket-water temperature triggers the alarm response?", "MX-02-engine-temperature"),
        new("Who must be informed about an engine temperature alarm?", "MX-02-engine-temperature"),
        new("Where is the spare thermostatic valve kit?", "VL-204-maintenance-note"),
        new("How overdue is the VL-204 thermostatic valve overhaul?", "VL-204-maintenance-note"),
        new("What is the captain's personal telephone number?", "")
    ];

    public static bool IsHit(GroundTruth truth, IEnumerable<VesselChunk> results) =>
        truth.ExpectedTitle.Length > 0 && results.Take(SearchDemo.TopK).Any(document => document.Title == truth.ExpectedTitle);

    public static async Task RunAsync(CancellationToken cancellationToken)
    {
        GroundTruth[] answerable = Questions.Where(question => question.ExpectedTitle.Length > 0).ToArray();
        if (answerable.Length == 0) throw new InvalidOperationException("Evaluation requires answerable ground-truth questions.");
        GeneratedEmbeddings<Embedding<float>> vectors = await EmbeddingFactory.Shared.GenerateAsync(
            answerable.Select(question => question.Question), EmbeddingFactory.Options, cancellationToken);
        if (vectors.Count != answerable.Length || vectors.Any(vector => vector.Vector.Length != AppConfig.Embedding.Dimensions))
            throw new InvalidOperationException("Evaluation embedding count or dimension mismatch.");
        foreach (string mode in new[] { "Text", "Vector", "Hybrid" })
        {
            int hits = 0;
            for (int index = 0; index < answerable.Length; index++)
            {
                List<VesselChunk> documents = await SearchDemo.RetrieveAsync(mode == "Vector" ? null : answerable[index].Question,
                    mode == "Text" ? null : vectors[index].Vector, cancellationToken);
                if (IsHit(answerable[index], documents)) hits++;
            }
            Console.WriteLine($"{mode}: hit@3 = {hits}/{answerable.Length} ({100.0 * hits / answerable.Length:F0}%)");
        }
        Console.WriteLine("Unanswerable questions are excluded from hit@3; test abstention separately with ask.");
    }
}