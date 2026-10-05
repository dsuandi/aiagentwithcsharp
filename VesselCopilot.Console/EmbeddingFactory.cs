using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;

public static class EmbeddingFactory
{
    private static IEmbeddingGenerator<string, Embedding<float>>? _shared;
    public static IEmbeddingGenerator<string, Embedding<float>> Shared => _shared ??= Create();
    public static EmbeddingGenerationOptions Options => new() { Dimensions = AppConfig.Embedding.Dimensions };

    private static IEmbeddingGenerator<string, Embedding<float>> Create()
    {
        if (AppConfig.Embedding.Dimensions < 1)
            throw new InvalidOperationException("Embedding:Dimensions must be positive.");
        Uri endpoint = AppConfig.RequireHttpsUri("Foundry:OpenAIEndpoint", AppConfig.Foundry.OpenAIEndpoint);
        string deployment = AppConfig.Require("Embedding:DeploymentName", AppConfig.Embedding.DeploymentName);
        return new AzureOpenAIClient(endpoint, CredentialFactory.Credential)
            .GetEmbeddingClient(deployment).AsIEmbeddingGenerator();
    }

    public static async Task<ReadOnlyMemory<float>> VectorAsync(string text, CancellationToken cancellationToken)
    {
        ReadOnlyMemory<float> vector = await Shared.GenerateVectorAsync(text, Options, cancellationToken);
        if (vector.Length != AppConfig.Embedding.Dimensions)
            throw new InvalidOperationException("Embedding dimensions do not match the configured index. Use a fresh index and re-embed.");
        return vector;
    }
}