# Stable Edition Validation

Validated on 2026-10-06 using the user's existing Azure resources and edited endpoints. No new Azure service or host was provisioned. The user assigned Search Service Contributor, Search Index Data Contributor, and Search Index Data Reader; no role assignments were performed by the assistant.

## Build And Package Gates

- Workshop local suite: 47 checks passed, including a new regression for keyword mode accidentally materializing an empty ReadOnlyMemory vector.
- Requested stable package versions restore and compile; locked restores pass. Workshop/agent and reference core resolved graphs contain zero prerelease packages.
- Full `reference/rag-app/app/app.sln` builds, including shared projects, Blazor frontend, backend, both test assemblies, document-preparation CLI, and Functions host.
- Workshop/agent and reference core vulnerability audits report no known vulnerable packages.
- Reference runtime tests are blocked by absent .NET 8; the attempted UI test run aborted before tests executed. Docker WSL integration is unavailable. MAUI's optional build is blocked by missing android and wasm-tools-net8 workloads.

## Live Lab 2 Results

| Check | Result |
| --- | --- |
| Embedding deployment | text-embedding-3-large returned the requested 1536 dimensions |
| Baseline ingestion | 3 chunks uploaded to vessel-docs |
| Keyword/vector/hybrid search | All three ran; vector/hybrid ranked MX-02 for the paraphrased engine question |
| Full hybrid-grounded answer | Watch logging answer cited HW-07-heavy-weather |
| Out-of-corpus question | Exact approved-documents abstention returned |
| Baseline hit@3 | Text 8/8, Vector 8/8, Hybrid 8/8 |
| Chunk-size experiment | 400 characters, 100 overlap, fresh vessel-docs-400 index, 5 chunks; all modes 8/8 |
| Quality judging | Primary and fallback both returned relevance 4, groundedness 5 |
| Indexed injection test | 4 chunks in isolated vessel-docs-inj; first response rejected by citation guard, subsequent response cited HW-07 and ignored injected command |

The injection file and generated output copies were removed afterwards. Baseline ingestion never contained the injection document. The three named training indexes remain available; no destructive cloud cleanup was run. Single model-judge and injection observations are not general security or accuracy guarantees.

The independent reference app's cloud-deployed UI, citation panel, PDF upload, and document/image pipeline are **not passed**: no Storage or application host is available. Its full web solution compiled, but that does not fulfill Lab 2.2's deployment checkpoint. Do not represent the entire workshop as fully certified while these prerequisites remain missing.

## Stable Compatibility Fixes

- The old project Responses adapter compiled but failed with MissingMethodException against OpenAI 2.14.0. Public constructors plus refreshing Entra BearerTokenPolicy fixed real model/embedding calls.
- Sampling overrides emitted by quality evaluators caused primary-model HTTP 400. Omitting Temperature and TopP made the primary judge pass.
- The keyword conditional converted null through ReadOnlyMemory's array conversion, creating an invalid empty-vector request. The regression failed before the fix and passed afterwards; empty vectors no longer produce vector queries.
- Live primary/fallback comparison, assessment, function calls, all agent modes, MCP protocol calls/model orchestration, and an approved sequential workflow passed after migration.

## Student Reproduction

```bash
dotnet restore VesselCopilot.Agents --locked-mode
dotnet run --project VesselCopilot.Tests -c Release
AZURE_TOKEN_CREDENTIALS=AzureCliCredential dotnet run --project VesselCopilot.Console -c Release -- ingest
AZURE_TOKEN_CREDENTIALS=AzureCliCredential dotnet run --project VesselCopilot.Console -c Release -- search "what to do when the engine runs hot"
AZURE_TOKEN_CREDENTIALS=AzureCliCredential dotnet run --project VesselCopilot.Console -c Release -- ask "What must the watch log during heavy weather?"
AZURE_TOKEN_CREDENTIALS=AzureCliCredential dotnet run --project VesselCopilot.Console -c Release -- eval
AZURE_TOKEN_CREDENTIALS=AzureCliCredential dotnet run --project VesselCopilot.Console -c Release -- quality
```

Run ingestion only against your designated training index. Use the branch source and STABLE-PACKAGES.md, not obsolete inline SDK snippets in the original guide.