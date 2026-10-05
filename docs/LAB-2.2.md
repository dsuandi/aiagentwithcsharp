# Lab 2.2: Reference App And Grounded Answers

The workshop console's full Search-to-answer and abstention checks passed on the existing Search service. The separate reference-app deployment/UI remains unverified: existing-resources-only authorization is unchanged, and Storage/application-host resources are unavailable. Do not run provisioning commands against the current environment.

1. Switch to `lab-2.2-grounded-rag`.
2. The complete patched reference source is included under `reference/rag-app`; no submodule initialization is needed. Read `WORKSHOP-PATCHES.md` for upstream commit, license provenance, and security fixes.
3. Read the reference app's own README and prerequisites. Its .NET 8 runtime, Docker, azd login, resource, and role requirements are separate from those of the .NET 10 console labs.
4. Build the core backend and its Blazor/shared dependencies with `dotnet build reference/rag-app/app/backend/MinimalApi.csproj -c Release --source https://api.nuget.org/v3/index.json`. This is the verified core build target. The full solution, optional functions/prepdocs, tests, and separate MAUI client are not certified.
5. After explicit resource and spending approval, follow the reference app's own deployment instructions. `az login` is not necessarily an `azd auth login`. Inspect `azure.yaml`, Bicep, SKUs, regions, model versions, and cleanup scope before deployment.
6. With an approved deployment, test an in-corpus question, open a citation, and compare Text, Vector, and Hybrid modes. Record the deployment URL and observed results; a successful build alone does not satisfy this checkpoint.
7. Build the workshop console project and run the local checks. Then run `dotnet run --project VesselCopilot.Console -c Release -- ask "What must the watch log during heavy weather?"` against your indexed training corpus.
8. Ask `What is the crew change schedule for VL-204 in December?` and verify the exact abstention sentence.
9. Where Search is unavailable, `grounding-check` uses the three local synthetic documents to exercise live generation and citation validation only. It does not test retrieval or the deployed reference app.

Retrieved content and metadata are escaped before entering source delimiters. Unknown or missing citations are rejected before display. These checks do not prove that every cited fact is faithful to its source, and delimiters are not a complete prompt-injection defense.