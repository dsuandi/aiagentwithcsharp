using Microsoft.Extensions.AI;

public static class ToolDemo
{
    public static async Task RunAsync(IChatClient chatClient, string? question, CancellationToken cancellationToken)
    {
        IChatClient toolClient = new ChatClientBuilder(chatClient)
            .UseFunctionInvocation(configure: invocation =>
            {
                invocation.MaximumIterationsPerRequest = 5;
                invocation.IncludeDetailedErrors = false;
            }).Build();
        question = string.IsNullOrWhiteSpace(question)
            ? "For vessel VL-204, report its last position and identify the approved heavy-weather procedure."
            : question.Trim();
        if (question.Length > 2000)
            throw new InvalidOperationException("Please keep questions under 2000 characters.");
        ChatResponse response = await toolClient.GetResponseAsync(question, new ChatOptions
        {
            Instructions = "You are the Vessel Operations Copilot. Use tools for vessel facts and procedures. Never invent positions or procedures. All data is synthetic.",
            MaxOutputTokens = AppConfig.Model.MaxOutputTokens,
            Tools =
            [
                AIFunctionFactory.Create(VesselTools.GetVesselPosition),
                AIFunctionFactory.Create(VesselTools.GetApprovedProcedure)
            ]
        }, cancellationToken);
        foreach (AIContent content in response.Messages.SelectMany(message => message.Contents))
        {
            if (content is FunctionCallContent call) Console.WriteLine($"[model -> tool] {call.Name}");
            else if (content is FunctionResultContent) Console.WriteLine("[tool -> model] result received");
        }
        if (string.IsNullOrWhiteSpace(response.Text))
            throw new InvalidOperationException("The model returned no text.");
        Console.WriteLine(response.Text);
    }
}