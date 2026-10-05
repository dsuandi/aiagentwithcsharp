using Azure.Core;
using OpenAI;
using System.ClientModel.Primitives;

namespace Shared.Services;

public static class ModelClientFactory
{
    public static OpenAIClient CreateAzure(Uri endpoint, TokenCredential credential) => new(
        new BearerTokenPolicy(credential, "https://cognitiveservices.azure.com/.default"),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint.AbsoluteUri.TrimEnd('/') + "/openai/v1/") });
}