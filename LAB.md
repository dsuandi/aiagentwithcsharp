# Lab 1.1: Provision And Connect

## Start Here

Complete the root README setup checklist. Use an instructor-approved existing Foundry project or follow the guide's portal provisioning steps after confirming budget and permissions. This checkpoint reuses an existing project; it does not claim that fresh provisioning has been tested.

1. Switch to `lab-1.1-provision-and-connect`.
2. Open `VesselCopilot.Console/appsettings.json`. Replace `Foundry:ProjectEndpoint` with your project endpoint, and set each profile's deployment name to the exact name in your portal. Model name and deployment name are not interchangeable.
3. Keep Day 2 Search and OpenAI placeholders until the relevant lab. Do not add credentials to the file.
4. Run `dotnet restore VesselCopilot.Console --locked-mode` and `dotnet build VesselCopilot.Console -c Release`.
5. Run `dotnet run --project VesselCopilot.Tests -c Release` for local configuration checks.
6. Run `dotnet run --project VesselCopilot.Console -c Release -- hello`.
7. Run `dotnet run --project VesselCopilot.Console -c Release -- hello --Model:ActiveProfile=Fallback`.
8. Run `dotnet run --project VesselCopilot.Console -c Release -- compare` and compare text, latency, and token usage.
9. Run `dotnet list VesselCopilot.Console package --vulnerable --include-transitive`.

Alternatively, use environment-variable overrides in Bash:

```bash
export Foundry__ProjectEndpoint="https://<resource>.services.ai.azure.com/api/projects/<project>"
export Model__Profiles__Fallback__DeploymentName="<actual-fallback-deployment>"
dotnet run --project VesselCopilot.Console -c Release -- hello --Model:ActiveProfile=Fallback
```

For the review environment, the fallback's actual deployment name was `workshop-primary`, backed by `gpt-5.4-mini`, version `2026-03-17`. Substitute your own deployment name. If multiple developer credential sources conflict, `AZURE_TOKEN_CREDENTIALS=AzureCliCredential` selects the existing Azure CLI login for the tested Azure.Identity version.

## Checkpoint Evidence

The Release build completed without warnings or errors. A live fallback call produced a two-sentence RAG explanation and reported 31 input and 64 output tokens. A call to the primary `gpt-6-luna` deployment hit the 60-second limit; primary inference is not certified. The package vulnerability audit reported no known vulnerable packages on 2026-10-06.

The application exits nonzero on failure. This corrects the guide's dispatcher, which printed some service errors but could still report success to scripts.