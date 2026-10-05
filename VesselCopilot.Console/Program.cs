using Microsoft.Extensions.AI;

try
{
    string[] positional = args.TakeWhile(argument => !argument.StartsWith("--", StringComparison.Ordinal)).ToArray();
    AppConfig.Load(args.Skip(positional.Length).ToArray());
    Console.WriteLine(AppConfig.Describe());
    using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(60));
    Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };
    using IChatClient chatClient = ChatClientFactory.Create();
    string mode = positional.FirstOrDefault()?.ToLowerInvariant() ?? "hello";
    switch (mode)
    {
        case "hello": await HelloDemo.RunAsync(chatClient, cancellation.Token); break;
        case "compare": await ModelComparison.RunAsync(cancellation.Token); break;
        default: Console.Error.WriteLine("Unknown mode. Use hello or compare."); Environment.ExitCode = 2; break;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine(ErrorHints.Describe(exception));
    Environment.ExitCode = 1;
}