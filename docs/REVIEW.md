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

Pending. No build, behavioral test, live model call, Search operation, deployed reference application, MCP exchange, workflow run, or Application Insights delivery is claimed yet.