# Workshop Review And Remediation

Review started on 2026-10-06 against the uploaded C# student guide, revised October 2026. This report distinguishes findings from verified fixes and separates compilation, local behavior, and live Azure behavior.

## Initial Findings

| Finding | Student Impact | Remediation | Status |
| --- | --- | --- | --- |
| No companion source repository was present. | Students cannot retrieve complete checkpoint projects. | Build cumulative projects and create one Git branch for each of the nine prescribed labs. | Pending |
| Unpinned prerelease packages are recommended. | Future restores can change APIs and break snippets. | Pin versions actually compiled and preserve restore lock files. | Pending |
| The guide combines .NET 8/10 with Visual Studio 2022 17.10+. | The stated editor minimum does not guarantee .NET 10 support. | Require an editor release compatible with the selected SDK; explain SDK versus runtime installation. | Documented in README |
| Contributor rights are presented alongside role assignment prerequisites. | Students may be unable to grant data-plane access. | Distinguish resource-management access, role-assignment permission, and service data-plane roles. | Documented in README |
| Example deployment names do not establish model availability. | Provisioning or inference may fail despite correct code. | Verify existing supported deployments and embedding dimensions before live tests. | Pending |
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
| 1.1 | Release build passed without warnings or errors | 10 configuration checks passed | Fallback `workshop-primary` passed (31/64 tokens); primary `gpt-6-luna` timed out at 60 seconds |
| 1.2 | Release compilation passed | 13 checks passed | Two streamed turns with follow-up context passed; typed assessment returned valid Medium risk |
| 1.3 | Release compilation passed | 20 checks passed | Default query invoked both functions and returned the exact synthetic position and HW-07 procedure |
| 2.1 | Release compilation passed | 28 checks passed | Embeddings passed at 3072 dimensions on existing `text-embedding-3-large`; Search ingestion/retrieval blocked |

## Verified Corrections

- Pinned `Microsoft.Agents.AI.Foundry` to `1.23.0-preview.260928.1` and `Azure.Identity` to `1.21.0`. Azure.Identity 1.17.0 conflicted with credential types in the adapter's Azure.Core 1.62.0 dependency; the corrected graph compiles.
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

## Resource Authorization

The user explicitly selected existing resources only. No new Search service or reference-app infrastructure may be provisioned. The selected subscription contains the Foundry resource/project and Application Insights, but no Azure AI Search service was listed. Labs 2.1-2.3 live Search checks and Lab 2.2 cloud deployment are blocked under this constraint; compilation and local checks are not substitutes for those checks.

Remaining labs, fresh provisioning, Search operations, deployed reference-app behavior, MCP, workflows, and telemetry delivery remain unverified.