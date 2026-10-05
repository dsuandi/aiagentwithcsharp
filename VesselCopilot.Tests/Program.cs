int passed = 0;
void Check(string name, Action check)
{
    check();
    Console.WriteLine($"PASS: {name}");
    passed++;
}
void Reject(Action action)
{
    try { action(); }
    catch (InvalidOperationException) { return; }
    throw new Exception("Expected validation to reject this value.");
}
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"Expected {expected}; got {actual}.");
}

Check("missing setting rejected", () => Reject(() => AppConfig.Require("test", null)));
Check("placeholder rejected", () => Reject(() => AppConfig.Require("test", "<resource>")));
Check("HTTP endpoint rejected", () => Reject(() => AppConfig.RequireHttpsUri("test", "http://example.org")));
Check("relative endpoint rejected", () => Reject(() => AppConfig.RequireHttpsUri("test", "/api/projects/test")));
Check("HTTPS endpoint accepted", () => Equal("example.org", AppConfig.RequireHttpsUri("test", "https://example.org").Host));
string[] valid = ["--Foundry:ProjectEndpoint=https://example.org/api/projects/test"];
Check("profile loads with command override", () => { AppConfig.Load(valid); Equal("Primary", AppConfig.ActiveProfile); });
Check("fallback profile override", () => { AppConfig.Load([.. valid, "--Model:ActiveProfile=Fallback"]); Equal("Fallback", AppConfig.ActiveProfile); });
Check("unknown profile rejected", () => Reject(() => AppConfig.Load([.. valid, "--Model:ActiveProfile=Missing"])));
Check("zero output tokens rejected", () => Reject(() => AppConfig.Load([.. valid, "--Model:Profiles:Primary:MaxOutputTokens=0"])));
Check("excess output tokens rejected", () => Reject(() => AppConfig.Load([.. valid, "--Model:Profiles:Primary:MaxOutputTokens=4001"])));
Check("invalid model risk rejected", () => Reject(() => VoyageAssessment.Validate(new("summary", [], [], "follow-up", "Critical"))));
Check("valid model assessment accepted", () => VoyageAssessment.Validate(new("summary", ["Weather"], [], "Review HW-07", "High")));
Check("history preserves system and complete turns", () =>
{
    List<Microsoft.Extensions.AI.ChatMessage> history = [new(Microsoft.Extensions.AI.ChatRole.System, "system")];
    for (int turn = 0; turn < 8; turn++)
    {
        history.Add(new(Microsoft.Extensions.AI.ChatRole.User, $"question {turn}"));
        history.Add(new(Microsoft.Extensions.AI.ChatRole.Assistant, $"answer {turn}"));
    }
    history.Add(new(Microsoft.Extensions.AI.ChatRole.User, "latest"));
    ChatLoop.TrimHistory(history);
    Equal(10, history.Count);
    Equal(Microsoft.Extensions.AI.ChatRole.System, history[0].Role);
    Equal(Microsoft.Extensions.AI.ChatRole.User, history[1].Role);
    Equal("latest", history[^1].Text);
});
Console.WriteLine($"{passed} checks passed. No Azure calls were made.");