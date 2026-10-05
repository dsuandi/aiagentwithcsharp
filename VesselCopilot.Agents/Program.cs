using Azure.AI.Projects;

try
{
    string[] positional = args.TakeWhile(argument => !argument.StartsWith("--", StringComparison.Ordinal)).ToArray();
    AppConfig.Load(args.Skip(positional.Length).ToArray(), typeof(Program).Assembly);
    Console.WriteLine(AppConfig.Describe());
    var projectClient = new AIProjectClient(new Uri(AppConfig.Foundry.ProjectEndpoint), CredentialFactory.Credential);
    using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(60));
    Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; cancellation.Cancel(); };
    string mode = positional.FirstOrDefault()?.ToLowerInvariant() ?? "hello";
    switch (mode)
    {
        case "hello": await AgentBasics.HelloAsync(projectClient, cancellation.Token); break;
        case "stream": await AgentBasics.StreamAsync(projectClient, cancellation.Token); break;
        case "session": await AgentBasics.SessionAsync(projectClient, cancellation.Token); break;
        case "tools": await AgentBasics.ToolsAsync(projectClient, cancellation.Token); break;
        case "assess": await AgentBasics.AssessAsync(projectClient, cancellation.Token); break;
        case "approve": await ApprovalDemo.RunAsync(projectClient, cancellation.Token); break;
        case "mcp-check": await McpDemo.CheckAsync(cancellation.Token); break;
        case "mcp": await McpDemo.RunAsync(projectClient, cancellation.Token); break;
        default: Console.Error.WriteLine("Modes: hello, stream, session, tools, assess, approve, mcp, mcp-check."); Environment.ExitCode = 2; break;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine(ErrorHints.Describe(exception));
    Environment.ExitCode = 1;
}