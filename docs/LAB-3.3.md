# Lab 3.3: Sequential Workflow And Tracing

1. Switch to `lab-3.3-multi-agent-workflow`, restore the agent project in locked mode, and build in Release.
2. Run `dotnet run --project VesselCopilot.Tests -c Release`.
3. Run `dotnet run --project VesselCopilot.Agents -c Release -- workflow` against your confirmed model deployment.
4. Observe Intake, Knowledge, and Action in order. Review the procedure code and revision, then approve or reject the proposed synthetic draft. Check the final completion event and draft count.
5. Repeat with rejection. Confirm no draft is created, even if the model asks to retry. Unknown workflow requests are never automatically approved.
6. For Azure tracing, use an instructor-provided Application Insights instance. Store its connection string locally with `dotnet user-secrets set "APPLICATIONINSIGHTS_CONNECTION_STRING" "<connection-string>" --project VesselCopilot.Agents`, or set the same environment variable. Never commit or print it.
7. Repeat `workflow`, then inspect Application Insights for the `vessel.workflow` span and spans for all three instrumented agents. Exporter initialization alone is not evidence of telemetry arrival.

Live approved/rejected runs completed with synthetic draft counts 1/0. Application Insights confirmed the workflow, three agents, model calls, and both tool calls under operation `342dabe495282921572ec8a7c1d9184f`.

The core knowledge tool uses the guide's synthetic in-memory procedure library, so this workflow does not need Azure AI Search. Replacing it with Search retrieval is a stretch goal and remains blocked here.

Runs are capped at 120 seconds to cover three agents, tool calls, and approval. Human review time counts toward that limit; restart after a timeout. TracerProvider remains alive for the full process and is disposed on exit. Sensitive-data capture is disabled, and custom tags record only approval status and synthetic draft count.

This is a sequential workflow, not durable orchestration. Checkpoint persistence, resumability, concurrency, handoff variants, retries beyond the SDK policy, production identity deployment, and production evaluation are explicitly optional or outside the tested core checkpoint.