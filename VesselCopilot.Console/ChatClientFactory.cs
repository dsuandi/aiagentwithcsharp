using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;

public static class ChatClientFactory
{
    public static Uri Endpoint => new(
        AppConfig.RequireHttpsUri("Foundry:OpenAIEndpoint", AppConfig.Foundry.OpenAIEndpoint).AbsoluteUri.TrimEnd('/') + "/openai/v1/");

    public static OpenAIClient CreateOpenAIClient() => new(
        new BearerTokenPolicy(CredentialFactory.Credential, "https://cognitiveservices.azure.com/.default"),
        new OpenAIClientOptions { Endpoint = Endpoint });

    public static IChatClient Create(ModelProfile? profile = null) =>
        new ResponsesClient(
            new BearerTokenPolicy(CredentialFactory.Credential, "https://cognitiveservices.azure.com/.default"),
            new ResponsesClientOptions { Endpoint = Endpoint })
            .AsIChatClient((profile ?? AppConfig.Model).DeploymentName);
}