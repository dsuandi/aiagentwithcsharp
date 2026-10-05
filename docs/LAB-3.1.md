# Lab 3.1: Your First C# Agent

1. Switch to `lab-3.1-first-agent` and keep your confirmed Foundry endpoint and deployment settings.
2. Run `dotnet restore VesselCopilot.Agents --locked-mode` and `dotnet build VesselCopilot.Agents -c Release`.
3. Run `dotnet run --project VesselCopilot.Agents -c Release -- hello`, then `stream`.
4. Run `session`. The third answer should recall Rina's name and duty-operator role. Compare with a separate new process/session, which must not know that earlier conversation.
5. Run `tools`. Verify that the answer uses the synthetic position and approved procedure from Day 1.
6. Run `assess`. Confirm a typed, validated risk classification and follow-up recommendation.
7. Repeat selected modes with `--Model:ActiveProfile=Fallback` if the primary deployment times out.
8. Run `dotnet list VesselCopilot.Agents package --vulnerable --include-transitive`.

The agent project references the earlier console assembly to reuse helpers rather than duplicating them. It loads user secrets from its own project assembly (`vessel-copilot-agents-workshop`), not the console project's secrets. Both projects use the same nonsecret settings file. Individual demo runs are bounded to 60 seconds; rerun a failed demo rather than assuming it completed.