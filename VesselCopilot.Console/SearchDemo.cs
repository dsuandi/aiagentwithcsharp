using Azure.Search.Documents;
using Azure.Search.Documents.Models;

public static class SearchDemo
{
    public const int TopK = 3;
    public static SearchOptions Options(ReadOnlyMemory<float>? vector)
    {
        var options = new SearchOptions { Size = TopK };
        foreach (string field in new[] { "id", "title", "docType", "revision", "content" }) options.Select.Add(field);
        if (vector is not null)
            options.VectorSearch = new()
            {
                Queries = { new VectorizedQuery(vector.Value) { KNearestNeighborsCount = TopK, Fields = { "contentVector" } } }
            };
        return options;
    }

    public static async Task<List<VesselChunk>> RetrieveAsync(string? query, ReadOnlyMemory<float>? vector,
        CancellationToken cancellationToken)
    {
        SearchResults<VesselChunk> results = await SearchSetup.SharedSearchClient.SearchAsync<VesselChunk>(
            query, Options(vector), cancellationToken);
        var documents = new List<VesselChunk>();
        await foreach (SearchResult<VesselChunk> result in results.GetResultsAsync().WithCancellation(cancellationToken))
            documents.Add(result.Document);
        return documents;
    }

    public static async Task RunAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length > 2000)
            throw new InvalidOperationException("A search question of 1-2000 characters is required.");
        ReadOnlyMemory<float> vector = await EmbeddingFactory.VectorAsync(query, cancellationToken);
        foreach (string mode in new[] { "Keyword", "Vector", "Hybrid" })
        {
            Console.WriteLine(mode);
            List<VesselChunk> documents = await RetrieveAsync(mode == "Vector" ? null : query,
                mode == "Keyword" ? null : vector, cancellationToken);
            foreach (VesselChunk document in documents)
                Console.WriteLine($"{document.Id} (rev {document.Revision})");
        }
    }
}