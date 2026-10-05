using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

public static class ApprovalPrompt
{
    public static bool IsApproved(string? answer) => answer?.Trim() is "y" or "Y";

    public static async Task<bool> AskAsync(FunctionCallContent call, CancellationToken cancellationToken,
        TextReader? input = null, TextWriter? output = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        input ??= Console.In;
        output ??= Console.Out;
        output.WriteLine($"APPROVAL NEEDED: {call.Name}");
        output.WriteLine(System.Text.Json.JsonSerializer.Serialize(call.Arguments));
        output.Write("Approve? (y/N): ");
        string? answer = await Task.Run(input.ReadLine, CancellationToken.None).WaitAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        bool approved = IsApproved(answer);
        output.WriteLine($"[audit] {DateTime.UtcNow:O} user={Environment.UserName} tool={call.Name} decision={(approved ? "APPROVED" : "DENIED")}");
        return approved;
    }
}

public static class ApprovalDemo
{
    public static async Task RunAsync(AIProjectClient projectClient, CancellationToken cancellationToken)
    {
        AIFunction tool = new ApprovalRequiredAIFunction(AIFunctionFactory.Create(VesselTools.CreateMaintenanceDraft));
        AIAgent agent = projectClient.AsAIAgent(model: AppConfig.Model.DeploymentName, name: "VesselOpsAgent",
            instructions: AgentBasics.Instructions,
            tools: [AIFunctionFactory.Create(VesselTools.GetVesselPosition), tool]);
        AgentSession session = await agent.CreateSessionAsync(cancellationToken);
        AgentResponse response = await agent.RunAsync("VL-204 reported jacket-water temperature swings on the port main engine. Create a maintenance draft for an inspection.",
            session, AgentBasics.Options, cancellationToken);
        for (int round = 0; round < 3; round++)
        {
            var requests = response.Messages.SelectMany(message => message.Contents).OfType<ToolApprovalRequestContent>().ToList();
            if (requests.Count == 0)
            {
                Console.WriteLine(response.Text);
                Console.WriteLine($"Synthetic drafts created this run: {VesselTools.DraftCount}");
                return;
            }
            var answers = new List<AIContent>();
            bool denied = false;
            foreach (ToolApprovalRequestContent request in requests)
            {
                bool approved = request.ToolCall is FunctionCallContent call && await ApprovalPrompt.AskAsync(call, cancellationToken);
                denied |= !approved;
                answers.Add(request.CreateResponse(approved));
            }
            response = await agent.RunAsync(new ChatMessage(ChatRole.User, answers), session,
                AgentBasics.Options, cancellationToken);
            if (denied)
            {
                Console.WriteLine(response.Text);
                Console.WriteLine($"Approval denied; no automatic re-approval. Synthetic drafts created this run: {VesselTools.DraftCount}");
                return;
            }
        }
        throw new InvalidOperationException("Approval round limit reached; no further tools were approved.");
    }
}