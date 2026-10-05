using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

public static class ModelComparison
{
    public static async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (IConfigurationSection section in AppConfig.Config.GetSection("Model:Profiles").GetChildren())
        {
            ModelProfile profile = AppConfig.GetProfile(section.Key);
            using IChatClient client = ChatClientFactory.Create(profile);
            var stopwatch = Stopwatch.StartNew();
            ChatResponse response = await client.GetResponseAsync(
                "In two sentences, what should an operator check first in force 7 winds?",
                new ChatOptions { MaxOutputTokens = profile.MaxOutputTokens }, cancellationToken);
            stopwatch.Stop();
            Console.WriteLine($"{section.Key}: {profile.DeploymentName}; {stopwatch.ElapsedMilliseconds} ms; " +
                $"tokens {response.Usage?.InputTokenCount}/{response.Usage?.OutputTokenCount}");
            Console.WriteLine(response.Text);
        }
    }
}