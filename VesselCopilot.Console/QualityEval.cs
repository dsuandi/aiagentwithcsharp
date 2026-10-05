using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;

public static class QualityEval
{
    public static async Task RunAsync(IChatClient chatClient, CancellationToken cancellationToken)
    {
        const string question = "How often must the watch log position during heavy weather?";
        GroundedResult grounded = await GroundedAnswer.AskAsync(chatClient, question, cancellationToken);
        IChatClient judge = chatClient.AsBuilder().ConfigureOptions(options =>
        {
            options.Temperature = null;
            options.TopP = null;
        }).Build();
        var configuration = new ChatConfiguration(judge);
        List<ChatMessage> messages = [new(ChatRole.User, question)];
        var response = new ChatResponse(new ChatMessage(ChatRole.Assistant, grounded.Answer));
        EvaluationResult relevance = await new RelevanceEvaluator().EvaluateAsync(messages, response,
            configuration, cancellationToken: cancellationToken);
        EvaluationResult groundedness = await new GroundednessEvaluator().EvaluateAsync(messages, response,
            configuration, additionalContext: [new GroundednessEvaluatorContext(grounded.Sources)], cancellationToken: cancellationToken);
        Console.WriteLine($"Relevance: {relevance.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName).Value}");
        Console.WriteLine($"Groundedness: {groundedness.Get<NumericMetric>(GroundednessEvaluator.GroundednessMetricName).Value}");
    }
}