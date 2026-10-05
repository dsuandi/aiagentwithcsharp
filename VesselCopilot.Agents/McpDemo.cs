using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

public static class McpDemo
{
    public static async Task<McpClient> ConnectAsync(CancellationToken cancellationToken)
    {
        string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name ?? "Release";
        string serverDll = AppConfig.Config["Mcp:ServerDll"] ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            "../../../../VesselMcpServer/bin", configuration, "net10.0/VesselMcpServer.dll"));
        if (!File.Exists(serverDll))
            throw new InvalidOperationException("Build the MCP server first or set Mcp:ServerDll to its built DLL.");
        return await McpClient.CreateAsync(new StdioClientTransport(new()
        {
            Name = "VesselMcp",
            Command = "dotnet",
            Arguments = [serverDll]
        }), cancellationToken: cancellationToken);
    }

    public static async Task CheckAsync(CancellationToken cancellationToken)
    {
        await using McpClient client = await ConnectAsync(cancellationToken);
        var tools = await client.ListToolsAsync(cancellationToken: cancellationToken);
        if (tools.Count != 1 || tools[0].Name != "get_vessel_voyage")
            throw new InvalidOperationException("Unexpected MCP tool inventory.");
        foreach (string vesselId in new[] { "VL-204", "VL-99X", "VL-305" })
        {
            var result = await client.CallToolAsync("get_vessel_voyage", new Dictionary<string, object?> { ["vesselId"] = vesselId },
                cancellationToken: cancellationToken);
            string text = string.Join(" ", result.Content.OfType<TextContentBlock>().Select(content => content.Text));
            string expected = vesselId == "VL-204" ? "Singapore to Tanjung Priok" : vesselId == "VL-99X" ? "Error:" : "No voyage found";
            if (result.IsError == true || !text.Contains(expected, StringComparison.Ordinal))
                throw new InvalidOperationException("MCP protocol check failed.");
            Console.WriteLine($"PASS MCP: {vesselId}: {text}");
        }
    }

    public static async Task RunAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        await using McpClient client = await ConnectAsync(cancellationToken);
        var tools = (await client.ListToolsAsync(cancellationToken: cancellationToken))
            .Where(tool => tool.Name == "get_vessel_voyage").ToList();
        if (tools.Count != 1) throw new InvalidOperationException("The approved MCP voyage tool is missing.");
        Console.WriteLine("MCP tools: get_vessel_voyage");
        AIAgent agent = new ChatClientAgent(ChatClientFactory.Create(), name: "VesselOpsAgent",
            instructions: AgentBasics.Instructions,
            tools: [.. tools.Cast<AITool>(), AIFunctionFactory.Create(VesselTools.GetVesselPosition)]);
        Console.WriteLine((await agent.RunAsync("Where is VL-204 now, and what is its current voyage and ETA?",
            options: AgentBasics.Options, cancellationToken: cancellationToken)).Text);
    }
}