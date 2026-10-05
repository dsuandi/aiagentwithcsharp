using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

public static class SearchSetup
{
    private static SearchIndexClient? _indexClient;
    private static SearchClient? _searchClient;
    public static string IndexName => AppConfig.Require("Search:IndexName", AppConfig.Search.IndexName);
    public static SearchIndexClient SharedIndexClient => _indexClient ??= new(
        AppConfig.RequireHttpsUri("Search:Endpoint", AppConfig.Search.Endpoint), CredentialFactory.Credential);
    public static SearchClient SharedSearchClient => _searchClient ??= SharedIndexClient.GetSearchClient(IndexName);

    public static SearchIndex BuildIndex()
    {
        if (AppConfig.Embedding.Dimensions < 1)
            throw new InvalidOperationException("Embedding:Dimensions must be positive.");
        return new SearchIndex(IndexName)
        {
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
                new SearchableField("title") { IsFilterable = true },
                new SimpleField("docType", SearchFieldDataType.String) { IsFilterable = true },
                new SimpleField("revision", SearchFieldDataType.String) { IsFilterable = true },
                new SearchableField("content"),
                new VectorSearchField("contentVector", AppConfig.Embedding.Dimensions, "vprofile")
            },
            VectorSearch = new VectorSearch
            {
                Profiles = { new VectorSearchProfile("vprofile", "hnsw") },
                Algorithms = { new HnswAlgorithmConfiguration("hnsw") }
            }
        };
    }

    public static async Task CreateIndexAsync(CancellationToken cancellationToken) =>
        await SharedIndexClient.CreateOrUpdateIndexAsync(BuildIndex(), cancellationToken: cancellationToken);
}