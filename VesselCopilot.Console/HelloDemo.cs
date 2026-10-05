using Microsoft.Extensions.AI;

public static class HelloDemo
{
    public static async Task RunAsync(IChatClient chatClient, CancellationToken cancellationToken)
    {
        ChatResponse response = await chatClient.GetResponseAsync(
            "Explain retrieval-augmented generation.",
            new ChatOptions
            {
                Instructions = "You are an Azure assistant. Answer in no more than two sentences.",
                MaxOutputTokens = AppConfig.Model.MaxOutputTokens
            }, cancellationToken);
        if (string.IsNullOrWhiteSpace(response.Text))
            throw new InvalidOperationException("The model returned no text. Check output token limits and model settings.");
        Console.WriteLine(response.Text);
        Console.WriteLine($"Tokens in/out: {response.Usage?.InputTokenCount}/{response.Usage?.OutputTokenCount}");
    }
}