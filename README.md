# Develop AI Apps and Agents on Azure: C# Workshop

Three days of code-first, level 200-300 instruction for C# developers. Build a Vessel Operations Copilot using Microsoft Foundry, Microsoft.Extensions.AI, Azure AI Search, and Microsoft Agent Framework. All vessels, positions, procedures, and maintenance records are synthetic training data. This is a learning environment, not a production system or operational navigation tool.

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
2. **Install .NET.** Use the .NET SDK required by the checked-out project. The review environment currently has .NET SDK 10.0.112. Run `dotnet --info` and check the SDK list, not just the runtime list. A .NET 10 SDK does not by itself supply a .NET 8 runtime.
3. **Install an editor.** Use VS Code with C# Dev Kit, or a Visual Studio release that supports the selected SDK. Do not assume Visual Studio 2022 17.10 supports .NET 10.
4. **Install Git and Azure CLI.** Verify `git --version` and `az version`. Day 2 Lab 2.2 also requires Azure Developer CLI (`azd version`) and a working Docker engine (`docker info`), subject to the reference application's current prerequisites.
5. **Sign in.** Run `az login`, then `az account show --output table`. If needed, select your instructor-approved subscription with `az account set --subscription <subscription-id>`. C# applications must run in the same user environment as that login.
6. **Confirm permissions.** Contributor alone cannot generally assign roles. An administrator must grant the required resource-management and role-assignment permissions, or pre-provision the lab resources and data-plane roles. Follow the actual service's role requirements; management-plane access is not proof of model or Search access.
7. **Confirm model capacity.** Ask the instructor for supported chat and embedding model deployments, exact deployment names, compatible region, quota, and embedding dimensions. Names printed in the guide are not evidence of deployment availability. Do not deploy a model based solely on an example name.
8. **Record nonsecret settings.** Obtain the Foundry project endpoint (including `/api/projects/<project>`), OpenAI endpoint, chat deployment names, embedding deployment and dimensions, Search endpoint and index name. Never substitute the project endpoint for the OpenAI endpoint or vice versa.
9. **Protect secrets.** Never commit API keys, access tokens, connection strings, tenant-specific configuration, or student data. Use environment variables or `dotnet user-secrets` for local settings; user secrets are not encrypted and are not a production secret store. Do not paste secrets into chat.
10. **Get the lab source.** Clone the instructor's published repository when available. This review starts as a local repository; a local branch is not downloadable by students until the instructor publishes the repository or supplies a Git bundle.
11. **Check dependencies.** Follow each lab's pinned package versions, restore dependencies, and run its documented preflight and tests before making live calls. Do not replace pinned prerelease versions with an untested latest version.
12. **Prepare safe data and cleanup.** Use only the synthetic dataset. Confirm who owns each resource and who may delete it. Agree on a spending limit and a cleanup time before the workshop.

### Shell Conventions

Commands in this repository use Bash unless a section explicitly says otherwise. On Windows, use an appropriate terminal or translate environment-variable commands to PowerShell. Configuration environment variables use double underscores, for example `Model__ActiveProfile`; a colon is used in configuration keys and command-line overrides.

## Lab Branches

Each branch is intended to contain the complete cumulative source needed at that checkpoint. Use `git switch <branch>` in a clean working tree; run commands from the project folder specified by the lab. Branch names below are planned until the validation report confirms creation.

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

## Validation And Review

Validation is in progress. No lab is yet certified as building or running. The review will record package versions, build outcomes, deterministic tests, live Azure checks, instruction corrections, and remaining blockers in `docs/REVIEW.md`. A local test or a mocked response does not prove live model inference, deployed RAG behavior, or telemetry arrival in Azure.

## Cleanup

Clear local user secrets in each project when finished: `dotnet user-secrets clear`. Remove temporary role assignments through your instructor. For a reference app deployed with azd, inspect its environment before running `azd down --purge`. Delete only your personally owned workshop resources after confirming the resource group contains nothing shared. Never automatically delete the current subscription's resources.