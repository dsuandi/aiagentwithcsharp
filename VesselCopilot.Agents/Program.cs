using Azure.AI.Projects;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Trace;

try
{
    string[] positional = args.TakeWhile(argument => !argument.StartsWith("--", StringComparison.Ordinal)).ToArray();
    AppConfig.Load(args.Skip(positional.Length).ToArray(), typeof(Program).Assembly);
    Console.WriteLine(AppConfig.Describe());
    string? connectionString = AppConfig.Config["APPLICATIONINSIGHTS_CONNECTION_STRING"];
    using TracerProvider? tracerProvider = string.IsNullOrWhiteSpace(connectionString) ? null :
        Sdk.CreateTracerProviderBuilder().AddSource("*Microsoft.Agents.AI*").AddSource("VesselCopilot")
            .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString).Build();
    Console.WriteLine(tracerProvider is null ? "Azure tracing disabled: no connection string configured." : "Azure tracing enabled; sensitive data capture is disabled.");
    var projectClient = new AIProjectClient(new Uri(AppConfig.Foundry.ProjectEndpoint), CredentialFactory.Credential);
    using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(120));
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
        case "workflow": await VesselWorkflow.RunAsync(projectClient, cancellation.Token); break;
        default: Console.Error.WriteLine("Modes: hello, stream, session, tools, assess, approve, mcp, mcp-check, workflow."); Environment.ExitCode = 2; break;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine(ErrorHints.Describe(exception));
    Environment.ExitCode = 1;
}