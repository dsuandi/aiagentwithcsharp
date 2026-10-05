using Microsoft.Extensions.AI;

try
{
    string[] positional = args.TakeWhile(argument => !argument.StartsWith("--", StringComparison.Ordinal)).ToArray();
    AppConfig.Load(args.Skip(positional.Length).ToArray());
    Console.WriteLine(AppConfig.Describe());
    using var cancellation = new CancellationTokenSource();
    Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };
    using var request = CancellationTokenSource.CreateLinkedTokenSource(cancellation.Token);
    request.CancelAfter(TimeSpan.FromSeconds(60));
    using IChatClient chatClient = ChatClientFactory.Create();
    string mode = positional.FirstOrDefault()?.ToLowerInvariant() ?? "hello";
    switch (mode)
    {
        case "hello": await HelloDemo.RunAsync(chatClient, request.Token); break;
        case "compare": await ModelComparison.RunAsync(request.Token); break;
        case "chat": await ChatLoop.RunAsync(chatClient, cancellation.Token); break;
        case "assess": await VoyageAssessment.RunAsync(chatClient, request.Token); break;
        case "tools": await ToolDemo.RunAsync(chatClient, string.Join(" ", positional.Skip(1)), request.Token); break;
        case "ingest": await Ingest.RunAsync(cancellation.Token); break;
        case "search": await SearchDemo.RunAsync(positional.Length > 1 ? string.Join(" ", positional.Skip(1)) : "heavy weather lashing", request.Token); break;
        case "embedding-check":
            var vector = await EmbeddingFactory.VectorAsync("Synthetic HW-07 heavy-weather procedure", request.Token);
            Console.WriteLine($"Embedding dimensions verified: {vector.Length}");
            break;
        default: Console.Error.WriteLine("Unknown mode. Use hello, compare, chat, assess, tools, ingest, search, or embedding-check."); Environment.ExitCode = 2; break;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine(ErrorHints.Describe(exception));
    Environment.ExitCode = 1;
}