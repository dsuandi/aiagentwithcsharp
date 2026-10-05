# Workshop Review And Remediation

Review started on 2026-10-06 against the uploaded C# student guide, revised October 2026. This report distinguishes findings from verified fixes and separates compilation, local behavior, and live Azure behavior.

**Latest edition:** the user-requested stable migration supersedes the initial package and missing-Search observations below. Read STABLE-PACKAGES.md and STABLE-VALIDATION.md for current versions and results. The earlier evidence is retained as history, not installation guidance.

## Initial Findings

| Finding | Student Impact | Remediation | Status |
| --- | --- | --- | --- |
| No companion source repository was present. | Students cannot retrieve complete checkpoint projects. | Build cumulative projects and create one Git branch for each of the nine prescribed labs. | Checkpoints created locally; final inventory in README |
| Unpinned prerelease packages are recommended. | Future restores can change APIs and break snippets. | Pin versions actually compiled and preserve restore lock files. | Fixed in reconstructed projects |
| The guide combines .NET 8/10 with Visual Studio 2022 17.10+. | The stated editor minimum does not guarantee .NET 10 support. | Require an editor release compatible with the selected SDK; explain SDK versus runtime installation. | Documented in README |
| Contributor rights are presented alongside role assignment prerequisites. | Students may be unable to grant data-plane access. | Distinguish resource-management access, role-assignment permission, and service data-plane roles. | Documented in README |
| Example deployment names do not establish model availability. | Provisioning or inference may fail despite correct code. | Verify existing supported deployments and embedding dimensions before live tests. | Verified primary, fallback, and embeddings; actual fallback name differs |
| A successful build is not evidence of live Azure operation. | Students may receive apparently tested samples whose cloud path has not run. | Record build, local tests, and live-service checks separately. | Applied review policy |

## Validation Plan

1. Read every core step and preserve each lab's learning objective; identify optional stretch goals separately.
2. Reconstruct the guide's shared configuration and safety helpers and validate startup settings locally.
3. Build Day 1 checkpoints; test conversation, structured output, and validated tools.
4. Build Day 2 checkpoints; test chunking and evaluation; inspect available Search and embedding resources.
5. Build Day 3 checkpoints; test approval, MCP protocol, workflow execution, and tracing configuration.
6. Run permitted live tests against identified existing resources. Do not create, delete, or deploy Azure infrastructure without authorization.
7. Commit each complete checkpoint on its named branch, maintain a title/branch/evidence table, and leave the repository on `main` with final documentation.

## Environment Evidence

- OS: Ubuntu 24.04, x64.
- Installed SDK: .NET 10.0.112.
- Installed runtime: Microsoft.NETCore.App and Microsoft.AspNetCore.App 10.0.12.
- Azure CLI: existing login successfully returned the selected subscription on 2026-10-06; no tokens or keys were requested or displayed.
- Original guide is preserved locally and excluded from Git by default; publishing rights have not been assessed.

## Results

| Lab | Build | Local Checks | Live Check |
| --- | --- | --- | --- |
| 1.1 | Release build passed without warnings or errors | 10 configuration checks passed | Fallback hello passed; final comparison passed on both deployments with AzureCliCredential selected; initial primary timeout retained below |
| 1.2 | Release compilation passed | 13 checks passed | Two streamed turns with follow-up context passed; typed assessment returned valid Medium risk |
| 1.3 | Release compilation passed | 20 checks passed | Default query invoked both functions and returned the exact synthetic position and HW-07 procedure |
| 2.1 | Release compilation passed | 28 checks passed | Embeddings passed at 3072 dimensions on existing `text-embedding-3-large`; Search ingestion/retrieval blocked |
| 2.2 | Console and patched reference backend/frontend build passed | 33 grounding checks at initial checkpoint | Local-source generation cited HW-07 and abstained; full Search-to-answer and deployed reference-app checks blocked |
| 2.3 | Core and optional quality-evaluation compilation passed | 37 checks passed | Search-dependent hit@3, before/after measurements, injection retrieval, and model-judge quality checks blocked |
| 3.1 | Release build passed without warnings or errors | Reuses validated shared helpers | All five modes passed live on fallback: hello, stream, session, tools, assess |
| 3.2 | Approval, agent, and MCP server compilation passed | 41 checks and three real MCP calls passed | Scripted y created one synthetic draft; empty input created zero; live agent combined local and MCP facts |
| 3.3 | Release compilation passed, including workflow and exporter | 46 checks passed on final source, including approval visibility and cancellation | Approved/rejected workflows completed with draft counts 1/0; Azure confirmed workflow, all agents, model, and both tool spans |

Counts record checks available at each initial checkpoint. The final suite has 46 checks; early branches intentionally contain only their lab's code and checks. These are executable console checks run with `dotnet run`, not an xUnit suite discovered by `dotnet test`.

### Live Evidence

- Primary/fallback comparison: `workshop-primary` returned 22 input/52 output tokens in 4540 ms; `gpt-6-luna` returned 22/251 tokens in 4508 ms. These single-run observations are not performance benchmarks.
- The first primary call timed out with unrestricted DefaultAzureCredential. The comparison later succeeded with `AZURE_TOKEN_CREDENTIALS=AzureCliCredential`. This establishes a working configuration, not proof that the credential chain caused the timeout.
- Embeddings: existing `text-embedding-3-large` returned 3072 dimensions.
- Grounded generation used the three local synthetic documents, cited `[HW-07-heavy-weather]`, and returned the exact abstention for crew scheduling. No Search query occurred in those two checks.
- Workflow operation `342dabe495282921572ec8a7c1d9184f` is present in Application Insights. Safe metadata queries confirmed successful workflow, Intake, Knowledge, Action, model-call, SearchKnowledgeBase, and CreateMaintenanceDraft spans under that operation. No connection string, prompt, or response content was queried or committed.
- At the Application Insights component query scope, use `dependencies`/`requests`, `timestamp`, and `operation_Id`. Workspace-scoped queries use `AppDependencies`/`AppRequests` and workspace field names. The initial wrong-scope query failed; the corrected query succeeded.

## Verified Corrections

- The initial review pinned and verified a prerelease Foundry adapter. The current edition replaces it with stable 1.5.0 and uses the explicitly requested stable graph; see STABLE-PACKAGES.md rather than reproducing the historical setup.
- Explicitly opted into the guide's experimental Responses API with `OPENAI001`; other diagnostics remain enabled.
- Replaced the deprecated managed identity constructor with `ManagedIdentityId.SystemAssigned`.
- Separated positional modes/questions from configuration arguments before calling `AddCommandLine`. This also prevents settings overrides from entering a model prompt.
- Bounded the hello and comparison calls with cancellation and made errors return nonzero exit codes. In the original guide, hello/comparison calls did not receive the dispatcher's cancellation token.
- Confirmed that model and deployment names differ in the actual environment. The fallback model exists under deployment `workshop-primary`; the guide's literal fallback deployment would produce a not-found error here.
- Audited Lab 1.1 dependencies: no known vulnerable packages were reported on 2026-10-06.
- Added output limits and cancellation to structured assessment and streaming; validated typed model values before use.
- Trimmed history by complete pairs so the retained conversation cannot begin with an orphaned assistant answer. The chat session itself is not limited to 60 seconds; each model turn is.
- Corrected vessel identifiers to `\AVL-[0-9]{3}\z`: the original regex accepted a final newline and Unicode decimal digits.
- Confirmed live invocation of both tools. Tool arguments and returned bodies are no longer printed in diagnostic logs.
- Reproduced the original chunker's oversized single-paragraph defect with a failing check, then made the same check pass using bounded overlapping character windows. Invalid overlaps are rejected.
- Made the synthetic data path independent of the current working directory and copied the dataset to build output.
- Passed requested dimensions to embedding generation and checked returned dimensions and batch counts before upload. The actual environment uses `text-embedding-3-large` (3072), not the guide's small deployment (1536).
- Checked individual Search upload results. Documented that merge-or-upload does not remove stale chunk keys after source removal or chunking changes.
- Escaped entire retrieved passages, not merely exact closing tags, and validated citation IDs before display. Generated citations are not proof of factual faithfulness.
- Preserved filename-based source IDs across ingestion, citations, and evaluation. The human-readable `Title:` metadata is not used as the canonical citation ID.
- Replaced the temporary RAG submodule with a complete licensed source snapshot under `reference/rag-app`, avoiding an unpublished patched submodule commit. Provenance is recorded in `reference/rag-app/WORKSHOP-PATCHES.md`.
- Supplied the complete ten-question ground-truth set, kept unanswerable questions out of hit@3, and tested top-three cutoff behavior. Three documents with k=3 are not a discriminating retrieval benchmark.
- Kept the injection fixture outside the default data folder and documented source/output cleanup, avoiding accidental indexing into the approved baseline corpus.
- Added bounded agent run options and model-output validation to every Lab 3.1 mode. The original agent examples omitted output limits and cancellation for several runs.
- Loaded user secrets from the entry project's assembly when helpers are shared, so the Day 3 project has its own secret scope.
- Launched the prebuilt MCP server DLL rather than `dotnet run`, preventing restore/build stdout from corrupting stdio JSON-RPC. Used an application-relative absolute path and an explicit tool-name allowlist.
- Tested MCP discovery and actual valid/invalid/unknown tool exchanges; corrected the checker to use MCP 2.2.0's `TextContentBlock` rather than an AI `TextContent`.
- Bounded approval rounds, defaulted EOF/empty input to deny, escaped displayed argument values, and used GUID draft IDs to avoid same-second collisions.
- Verified rejection produced zero drafts despite the model suggesting a retry. The application, not model text, stopped re-approval. Approval tests used scripted input on synthetic non-persisted drafts; no real work order was created.
- A review regression failed when the argument preview hid part of the description and passed after displaying the complete JSON-escaped payload. Approval is over the actual arguments, not a truncated summary.
- Replaced blocking approval input with a cancellation-aware wait and rechecked cancellation before approving. A deliberately blocked-reader test verifies cancellation ends the wait with zero state changes. CLI termination releases the background reader; do not reuse this console pattern in a hosted multi-user service.
- Instrumented all workflow agents with sensitive capture off and handled errors and incomplete streams without claiming completion.

## Resource Authorization

The user explicitly selected existing resources only. No new Search service or reference-app infrastructure may be provisioned. The selected subscription contains the Foundry resource/project and Application Insights, but no Azure AI Search service was listed. Labs 2.1-2.3 live Search checks and Lab 2.2 cloud deployment are blocked under this constraint; compilation and local checks are not substitutes for those checks.

## Reference Dependency Remediation

The initial official reference backend compiled but failed its vulnerability gate:

| Dependency | Initial Version | Finding | Corrected Version |
| --- | --- | --- | --- |
| Microsoft.SemanticKernel.Core | 1.17.2 | Critical GHSA-2ww3-72rp-wpp4 / CVE-2026-25592 | 1.71.0 |
| SixLabors.ImageSharp | 1.0.4 | Multiple high/moderate advisories, including GHSA-2cmq-823j-5qj8 | 2.1.13 |
| System.Text.Json | 8.0.4 | High GHSA-8g4q-xg66-9fp4 / CVE-2024-43485 | 10.0.2, required by patched Semantic Kernel Core |

Central transitive pinning applies the fixes. The reference keeps its existing connector and net8.0 target; one scoped CS0618 suppression permits the still-supported deprecated embedding interface. The patched backend, frontend, and shared projects build. The resolved core graph reports no known vulnerable packages, as do the custom console/agent and MCP graphs. Package audits are not a complete security review and do not certify optional MAUI/functions/prepdocs projects.

## Remaining Gaps

- Fresh Foundry/Search provisioning, new role assignments, and reference-app cloud deployment were not attempted: the user allowed existing resources only.
- No Search service exists, so ingestion, index contents, three-mode retrieval, full RAG, hit@3, before/after measurements, indexed injection defense, and optional model-judge results remain unverified.
- The reference core backend/frontend build passed using `--source https://api.nuget.org/v3/index.json`. The broader upstream solution restore stalled and was stopped; optional projects and upstream tests are not certified.
- Only .NET 10 runtime was initially installed. The reference targets .NET 8; process startup and net8.0 tests require a separate current .NET 8 runtime/SDK. Do not silently roll forward and claim normal .NET 8 behavior.
- Stretch goals are not certified: REST contract checks, durable checkpoints, handoff/concurrent variants, RAG-backed workflow knowledge, fallback orchestration, query rewriting, semantic ranker, and AI gateway deployment.
- The capstone remains a learner-selected design exercise. Production identity, data isolation, retention, injection protections, evaluation, budgets, and rollback require scenario-specific validation.
- Existing resources and temporary roles were not deleted or revoked. No destructive cleanup commands were run.

The repository is local, not published remotely. Instructor publication or a local clone is still needed for student distribution.