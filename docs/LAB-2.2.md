# Lab 2.2: Reference App And Grounded Answers

Cloud deployment and full Search-to-answer checks are blocked in this review: the user authorized existing resources only, and no Search service is available. Do not run provisioning commands against the current environment.

1. Switch to `lab-2.2-grounded-rag`.
2. Run `git submodule update --init --recursive`. The official reference app is stored as a submodule under `reference/azure-search-openai-demo-csharp`, pinned to the Git commit recorded in this branch. A ZIP download of this repository does not include submodule contents.
3. Read the pinned reference app's README and prerequisites. Its .NET 8 runtime, Docker, azd login, resource, and role requirements are separate from those of the .NET 10 console labs.
4. Build its web solution with `dotnet build reference/azure-search-openai-demo-csharp/app/app.sln -c Release`; do not build the separate MAUI solution for this lab.
5. After explicit resource and spending approval, follow the reference app's own deployment instructions. `az login` is not necessarily an `azd auth login`. Inspect `azure.yaml`, Bicep, SKUs, regions, model versions, and cleanup scope before deployment.
6. With an approved deployment, test an in-corpus question, open a citation, and compare Text, Vector, and Hybrid modes. Record the deployment URL and observed results; a successful build alone does not satisfy this checkpoint.
7. Build the workshop console project and run the local checks. Then run `dotnet run --project VesselCopilot.Console -c Release -- ask "What must the watch log during heavy weather?"` against your indexed training corpus.
8. Ask `What is the crew change schedule for VL-204 in December?` and verify the exact abstention sentence.
9. Where Search is unavailable, `grounding-check` uses the three local synthetic documents to exercise live generation and citation validation only. It does not test retrieval or the deployed reference app.

Retrieved content and metadata are escaped before entering source delimiters. Unknown or missing citations are rejected before display. These checks do not prove that every cited fact is faithful to its source, and delimiters are not a complete prompt-injection defense.