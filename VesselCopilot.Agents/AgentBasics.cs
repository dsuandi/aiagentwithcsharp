using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

public static class AgentBasics
{
    public const string Instructions = "You are the Vessel Operations Copilot for a shipping team. Answer briefly. Use tools for vessel facts; never invent positions or procedures. All workshop data is synthetic.";
    public static ChatClientAgentRunOptions Options => new()
    {
        ChatOptions = new ChatOptions { MaxOutputTokens = AppConfig.Model.MaxOutputTokens }
    };

    public static AIAgent CreateAgent(AIProjectClient projectClient, bool tools = false) =>
        projectClient.AsAIAgent(model: AppConfig.Model.DeploymentName, name: "VesselOpsAgent", instructions: Instructions,
            tools: tools ? [AIFunctionFactory.Create(VesselTools.GetVesselPosition), AIFunctionFactory.Create(VesselTools.GetApprovedProcedure)] : null);

    public static async Task HelloAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIAgent agent = CreateAgent(projectClient);
        AgentResponse response = await agent.RunAsync("In one sentence, what can you help a vessel operations team with?",
            options: Options, cancellationToken: cancellationToken);
        Console.WriteLine(response.Text);
    }

    public static async Task StreamAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIAgent agent = CreateAgent(projectClient);
        await foreach (AgentResponseUpdate update in agent.RunStreamingAsync("Give three tips for writing a clear noon report.",
            options: Options, cancellationToken: cancellationToken))
            Console.Write(update.Text);
        Console.WriteLine();
    }

    public static async Task SessionAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIAgent agent = CreateAgent(projectClient);
        AgentSession session = await agent.CreateSessionAsync(cancellationToken);
        foreach (string question in new[]
        {
            "My name is Rina and I am the duty operator today.",
            "VL-204 reports force 7 winds. What should I check first?",
            "Remind me: what is my name and role?"
        })
            Console.WriteLine((await agent.RunAsync(question, session, Options, cancellationToken)).Text);
    }

    public static async Task ToolsAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIAgent agent = CreateAgent(projectClient, tools: true);
        Console.WriteLine((await agent.RunAsync("For VL-204, report its last position and identify the approved heavy-weather procedure.",
            options: Options, cancellationToken: cancellationToken)).Text);
    }

    public static async Task AssessAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIAgent agent = CreateAgent(projectClient);
        AgentResponse<VoyageReportAssessment> response = await agent.RunAsync<VoyageReportAssessment>(
            "Assess this synthetic voyage report. RiskLevel must be Low, Medium or High.\n" + VoyageAssessment.SampleReport,
            options: Options, cancellationToken: cancellationToken);
        VoyageAssessment.Validate(response.Result);
        Console.WriteLine($"{response.Result.RiskLevel}: {response.Result.Summary}");
        Console.WriteLine($"Follow-up: {response.Result.RequiredFollowUp}");
    }
}