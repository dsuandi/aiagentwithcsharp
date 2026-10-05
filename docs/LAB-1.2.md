# Lab 1.2: Streaming, History, And Structured Output

Start from the Lab 1.1 settings and working model deployment. This branch includes all previous code.

1. Switch to `lab-1.2-streaming-chat`.
2. Run `dotnet restore VesselCopilot.Console --locked-mode`.
3. Run `dotnet build VesselCopilot.Console -c Release` and `dotnet run --project VesselCopilot.Tests -c Release`.
4. Run `dotnet run --project VesselCopilot.Console -c Release -- chat`.
5. Ask `VL-204 is reporting force 7 winds. What should the operator check first?` and then `Summarise that as three bullet points.` Check that text streams and the follow-up uses the conversation.
6. Type `exit`, or press Ctrl+D to close input. Use Ctrl+C to cancel an active model request.
7. Run `dotnet run --project VesselCopilot.Console -c Release -- assess`. Check the typed summary, topics, risks, follow-up, and validated Low/Medium/High risk level.
8. Repeat with `--Model:ActiveProfile=Fallback` after either mode, or use the environment override described in Lab 1.1.

Streaming and assessment requests use the configured output limit and a 60-second per-request timeout. History retains the system instruction and four complete turns plus the current question, rather than an orphaned assistant message. History trimming is deliberately simple; it is not a token estimator or summarizer.

Build, local-check, and live-check results are recorded in `REVIEW.md`. Structured output is untrusted model data, not authorization to take an operational action.