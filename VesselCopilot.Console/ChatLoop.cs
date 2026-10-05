using System.Text;
using Microsoft.Extensions.AI;

public static class ChatLoop
{
    public static void TrimHistory(List<ChatMessage> history)
    {
        while (history.Count > 10)
            history.RemoveRange(1, Math.Min(2, history.Count - 2));
    }

    public static async Task RunAsync(IChatClient chatClient, CancellationToken cancellationToken)
    {
        List<ChatMessage> history =
        [
            new(ChatRole.System, "You are the Vessel Operations Copilot. Answer briefly, admit uncertainty, and use only synthetic workshop data.")
        ];
        Console.WriteLine("Vessel Operations Copilot: type exit to quit.");
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("You: ");
            string? question = Console.ReadLine()?.Trim();
            if (question is null || question.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
            if (question.Length == 0) continue;
            if (question.Length > 2000)
            {
                Console.WriteLine("Please keep questions under 2000 characters.");
                continue;
            }
            history.Add(new ChatMessage(ChatRole.User, question));
            TrimHistory(history);
            using var turn = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            turn.CancelAfter(TimeSpan.FromSeconds(60));
            var response = new StringBuilder();
            Console.Write("Copilot: ");
            await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(history,
                new ChatOptions { MaxOutputTokens = AppConfig.Model.MaxOutputTokens }, turn.Token))
            {
                Console.Write(update.Text);
                response.Append(update.Text);
            }
            if (response.Length == 0)
                throw new InvalidOperationException("The model returned no text.");
            history.Add(new ChatMessage(ChatRole.Assistant, response.ToString()));
            Console.WriteLine();
        }
    }
}