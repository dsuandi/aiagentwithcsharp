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
Console.WriteLine($"{passed} checks passed. No Azure calls were made.");