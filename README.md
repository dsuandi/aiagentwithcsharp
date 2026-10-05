# Develop AI Apps and Agents on Azure: C# Workshop

Three days of code-first, level 200-300 instruction for C# developers. Build a Vessel Operations Copilot using Microsoft Foundry, Microsoft.Extensions.AI, Azure AI Search, and Microsoft Agent Framework. All vessels, positions, procedures, and maintenance records are synthetic training data. This is a learning environment, not a production system or operational navigation tool.

## Workshop Path

| Day | Focus | Student Output |
| --- | --- | --- |
| 1 | Model access and application fundamentals | A configurable C# console app with streaming conversation, structured assessments, and validated function tools |
| 2 | Grounding and measurement | A synthetic document corpus, ingestion and retrieval code, cited answers, and a repeatable retrieval-quality experiment |
| 3 | Agents and controlled actions | An agent with sessions and tools, a stdio MCP server, human-approved synthetic drafts, and a traced sequential workflow |

Finish core steps before stretch goals. Each lab has an independent cumulative source branch, so students can recover from a missed step or inspect a working checkpoint without copying incomplete snippets.

## Learning Outcomes

By the end of the workshop, students should be able to:

- Connect to a Foundry model using Microsoft Entra ID instead of API keys.
- Implement model calls, streaming conversations, bounded history, and typed responses in C#.
- Expose validated C# functions as model tools and explain the function-calling loop.
- Chunk and embed documents, index them in Azure AI Search, and compare keyword, vector, and hybrid retrieval.
- Generate grounded answers with citations and measure retrieval quality against a small ground-truth dataset.
- Build agents with sessions, function tools, structured output, human approval, and MCP tools.
- Compose a sequential multi-agent workflow and instrument it for Application Insights.
- Identify trust boundaries, reject invalid tool arguments, and document production gaps.

## Before The Workshop

Complete these steps before Day 1. Do not create chargeable resources until your instructor confirms the subscription, region, resource group, model availability, and budget.

1. **Check your skills.** Be comfortable with C# classes, async/await, NuGet, Git, and terminal commands. No prior machine-learning experience is required.
2. **Install .NET.** The reconstructed console, agent, MCP, and check projects target .NET 10. Install SDK 10.0.112 or a compatible later patch in its 10.0.1xx feature band; the committed global.json controls selection. Run `dotnet --info` from the repository and check SDKs, not just runtimes. The included reference app targets .NET 8: install a current .NET 8 SDK/runtime separately before running it or its tests. A .NET 10 SDK alone can build that app but does not supply its runtime.
3. **Install an editor.** Use VS Code with C# Dev Kit, or a Visual Studio release that supports the selected SDK. Do not assume Visual Studio 2022 17.10 supports .NET 10.
4. **Install Git and Azure CLI.** Verify `git --version` and `az version`. Day 2 Lab 2.2 also requires Azure Developer CLI (`azd version`) and a working Docker engine (`docker info`), subject to the reference application's current prerequisites.
5. **Sign in.** Run `az login`, then `az account show --output table`. If needed, select your instructor-approved subscription with `az account set --subscription <subscription-id>`. C# applications must run in the same user environment as that login.
6. **Confirm permissions.** Contributor alone cannot generally assign roles. An administrator must grant the required resource-management and role-assignment permissions, or pre-provision the lab resources and data-plane roles. Follow the actual service's role requirements; management-plane access is not proof of model or Search access.
7. **Confirm model capacity.** Ask the instructor for supported chat and embedding model deployments, exact deployment names, compatible region, quota, and embedding dimensions. Names printed in the guide are not evidence of deployment availability. Do not deploy a model based solely on an example name.
8. **Record nonsecret settings.** Obtain the Foundry project endpoint (including `/api/projects/<project>`), OpenAI endpoint, chat deployment names, embedding deployment and dimensions, Search endpoint and index name. Never substitute the project endpoint for the OpenAI endpoint or vice versa.
9. **Protect secrets.** Never commit API keys, access tokens, connection strings, tenant-specific configuration, or student data. Use environment variables or `dotnet user-secrets` for local settings; user secrets are not encrypted and are not a production secret store. Do not paste secrets into chat.
10. **Get the lab source.** Clone the instructor's repository and select the required branch. The complete patched Lab 2.2 reference source is included directly; no submodule initialization is needed. This repository is currently local and must be published or shared before students can retrieve it from another machine.
11. **Check dependencies.** Follow each lab's pinned package versions, restore dependencies, and run its documented preflight and tests before making live calls. Do not replace pinned prerelease versions with an untested latest version.
12. **Prepare safe data and cleanup.** Use only the synthetic dataset. Confirm who owns each resource and who may delete it. Agree on a spending limit and a cleanup time before the workshop.

### Shell Conventions

Commands in this repository use Bash unless a section explicitly says otherwise. On Windows, use an appropriate terminal or translate environment-variable commands to PowerShell. Configuration environment variables use double underscores, for example `Model__ActiveProfile`; a colon is used in configuration keys and command-line overrides.

## Lab Branches

All nine branches below exist in the local repository and contain complete cumulative source for their checkpoint. Every branch has been checked out, compiled, and run through its local checks. Use `git switch <branch>` in a clean working tree; follow that lab's instructions. Full Azure checks for Day 2 remain blocked as described below.

| Lab | Guide Title | Branch |
| --- | --- | --- |
| 1.1 | Provision and Connect: Your First Foundry Call from C# | `lab-1.1-provision-and-connect` |
| 1.2 | Build a Chat-Enabled C# App: Streaming, Chat History, Structured Output | `lab-1.2-streaming-chat` |
| 1.3 | Tool (Function) Calling in C# | `lab-1.3-function-calling` |
| 2.1 | Ingest and Index Your Data with Azure AI Search (C#) | `lab-2.1-ingest-and-index` |
| 2.2 | Deploy and Explore the End-to-End C# RAG Reference App | `lab-2.2-grounded-rag` |
| 2.3 | Improve and Evaluate Retrieval Quality | `lab-2.3-retrieval-evaluation` |
| 3.1 | Build Your First Agent in C# with the Microsoft Agent Framework | `lab-3.1-first-agent` |
| 3.2 | Extend Your Agent with Tools and an MCP Server | `lab-3.2-approval-and-mcp` |
| 3.3 | Multi-Agent Workflow with Human-in-the-Loop and Observability | `lab-3.3-multi-agent-workflow` |

The capstone is a scenario-design worksheet, not a tenth prescribed implementation. Select only the building blocks your scenario needs and record unmet production requirements.

## Get A Checkpoint

On this machine, a student can clone the complete local repository:

```bash
git clone /home/dsuandi/workshop ../workshop-student
cd ../workshop-student
git branch --all
git switch lab-1.1-provision-and-connect
```

For another machine, replace the local path with the instructor's published repository URL. Do not use a shallow single-branch clone when you need every checkpoint. No remote publication or push has been performed.

Start with [LAB.md](LAB.md) for Lab 1.1. Later checkpoints include their own `docs/LAB-<number>.md` guide. The shared [review report](docs/REVIEW.md) records all remediations and verification limits; use `main` for the complete final source and report.

After confirming deployment names and replacing placeholders, run:

```bash
dotnet restore VesselCopilot.Console --locked-mode
dotnet build VesselCopilot.Console -c Release
dotnet run --project VesselCopilot.Tests -c Release
dotnet run --project VesselCopilot.Console -c Release -- hello
```

From Lab 3.1 onward, build and run `VesselCopilot.Agents` instead. It references the tested console helpers; from Lab 3.2 it also builds the MCP server. From Lab 2.2, the verified reference core build is:

```bash
dotnet build reference/rag-app/app/backend/MinimalApi.csproj -c Release --source https://api.nuget.org/v3/index.json
```

The review's fallback deployment was named `workshop-primary` even though its model is `gpt-5.4-mini`; configure your own exact deployment name. `AZURE_TOKEN_CREDENTIALS=AzureCliCredential` selects the existing CLI login for the tested Azure.Identity version. Do not use an API key to work around missing data-plane permissions.

## Validation And Review

The final source passes 46 local checks. All nine checkpoint checkouts compiled and passed their respective checks. Live verification covered two-model comparison, streaming and typed output, function calling, agent sessions/tools, approval and rejection, real MCP exchanges, and sequential workflows. Application Insights confirmed correlated workflow, agent, model, and tool spans. The custom graphs and patched reference core passed dependency vulnerability audits.

Day 2 is **partially verified**, not fully certified: live embeddings and generation from local synthetic sources passed, but no Search service exists and the user authorized existing resources only. Search ingestion/retrieval, end-to-end RAG, retrieval measurements, indexed injection defense, cloud deployment, and optional quality scores remain unverified. The reference backend/frontend builds; its runtime and upstream tests additionally require .NET 8. See [docs/REVIEW.md](docs/REVIEW.md) for exact results and remediation.

## Cleanup

Clear local user secrets in each project when finished: `dotnet user-secrets clear`. Remove temporary role assignments through your instructor. For a reference app deployed with azd, inspect its environment before running `azd down --purge`. Delete only your personally owned workshop resources after confirming the resource group contains nothing shared. Never automatically delete the current subscription's resources.