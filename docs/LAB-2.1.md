# Lab 2.1: Ingest And Index

The stable edition was tested against the existing Search service after the user assigned its data-plane roles: three synthetic chunks uploaded and all retrieval modes ran successfully. See STABLE-VALIDATION.md for the latest evidence.

1. Switch to `lab-2.1-ingest-and-index` and retain the Day 1 settings.
2. Have the instructor provide a Search service with RBAC enabled. Index creation requires Search Service Contributor, upload requires Search Index Data Contributor, and read-only queries require Search Index Data Reader.
3. Set `Search:Endpoint`, `Search:IndexName`, `Foundry:OpenAIEndpoint`, `Embedding:DeploymentName`, and `Embedding:Dimensions` in configuration. Use the deployment's actual supported dimensions. Changing dimensions requires a fresh index and re-embedding.
4. Run `dotnet restore VesselCopilot.Console --locked-mode`, `dotnet build VesselCopilot.Console -c Release`, and `dotnet run --project VesselCopilot.Tests -c Release`.
5. Run `dotnet run --project VesselCopilot.Console -c Release -- embedding-check` to verify embedding access and dimension compatibility without uploading anything.
6. Review the three synthetic Markdown files under `VesselCopilot.Console/data`.
7. Run `dotnet run --project VesselCopilot.Console -c Release -- ingest` only against your designated training index. This creates or updates the index and uploads documents.
8. Inspect index fields and document count in the Search portal. Then run `dotnet run --project VesselCopilot.Console -c Release -- search "what to do when the engine runs hot"` and repeat with `"HW-07 revision"`.
9. Compare the top three keyword, vector, and hybrid results. Scores from different retrieval modes are not directly comparable confidence scores.

The corrected chunker uses bounded character windows with overlap. It is not token-aware. Data is copied to the application output folder, so commands work from the repository root as well as the project folder. Embeddings are generated in batches of 16 and uploads in batches of 500; upload results are checked individually.

Re-running unchanged documents overwrites matching keys. Changing chunk sizes or removing files does not delete old chunk keys: use a fresh training index for those experiments. Never delete or reuse a shared production index.