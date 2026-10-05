using Azure.AI.Projects;
using Microsoft.Extensions.AI;

public static class ChatClientFactory
{
    public static IChatClient Create(ModelProfile? profile = null) =>
        new AIProjectClient(new Uri(AppConfig.Foundry.ProjectEndpoint), CredentialFactory.Credential)
            .GetProjectOpenAIClient()
            .GetProjectResponsesClient()
            .AsIChatClient((profile ?? AppConfig.Model).DeploymentName);
}