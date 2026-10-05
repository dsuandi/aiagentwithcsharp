# Stable Package Edition

The current workshop and reference web solution use stable NuGet releases only. Package versions are pinned; no preview/beta package selection is needed. Stable NuGet status does not guarantee every API lacks an experimental annotation: `OPENAI001` explicitly opts into the stable SDK's bearer-policy/Responses API surface.

## Required Versions

| Package | Version |
| --- | --- |
| Azure.AI.OpenAI | 2.1.0 |
| Azure.AI.Projects | 2.0.1 |
| Azure.Identity | 1.21.0 |
| Microsoft.Agents.AI.Foundry | 1.5.0 |
| Microsoft.Extensions.AI | 10.10.0 |
| Microsoft.Extensions.AI.OpenAI | 10.10.1 |
| Microsoft.Extensions.Configuration.Binder | 10.0.12 |
| Microsoft.Extensions.Configuration.CommandLine | 10.0.12 |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 10.0.12 |
| Microsoft.Extensions.Configuration.Json | 10.0.12 |
| Microsoft.Extensions.Configuration.UserSecrets | 10.0.12 |

Use `dotnet add package <name> --version <version>` when following the manual setup steps. For a checkpoint clone, prefer `dotnet restore <project> --locked-mode`; changing already pinned package references is unnecessary.

The other tested direct workshop dependencies are Azure.Search.Documents 12.0.0, Microsoft.Extensions.AI.Evaluation.Quality 10.10.0, Microsoft.Agents.AI.Workflows 1.23.0, ModelContextProtocol 2.2.0, OpenTelemetry 1.19.1, Azure.Monitor.OpenTelemetry.Exporter 1.9.0, and Microsoft.Extensions.Hosting 10.0.0.

## Stable Client Pattern

The project adapter in Azure.AI.Projects 2.0.1 is binary-incompatible with the OpenAI 2.14.0 library required by Microsoft.Extensions.AI.OpenAI 10.10.1 when it constructs a Responses client. Compilation does not reveal this: the initial live call failed with MissingMethodException.

The corrected source uses public SDK constructors and refreshing Entra bearer authentication:

```csharp
var policy = new BearerTokenPolicy(
    CredentialFactory.Credential,
    "https://cognitiveservices.azure.com/.default");
var endpoint = new Uri(
    AppConfig.Foundry.OpenAIEndpoint.TrimEnd('/') + "/openai/v1/");
var client = new ResponsesClient(policy,
    new ResponsesClientOptions { Endpoint = endpoint });
IChatClient chat = client.AsIChatClient(AppConfig.Model.DeploymentName);
```

Use `ChatClientFactory.Create()` rather than the project Responses adapter. Use `new ChatClientAgent(chat, ...)` for local agents. Embeddings use `ChatClientFactory.CreateOpenAIClient().GetEmbeddingClient(deployment).AsIEmbeddingGenerator()`. All clients reuse the shared credential; API keys are not a compatibility workaround.

Configure both Foundry endpoints before Lab 1.1. The project endpoint includes `/api/projects/<project>`; the OpenAI resource endpoint is a different URL. In this environment the fallback deployment is `workshop-primary`, and `text-embedding-3-large` supports the explicitly requested 1536 dimensions. Neither deployment name is inferred from the model name.

## Reference Application

The reference backend no longer uses the old Semantic Kernel connector, whose updated Azure connector still requires a beta OpenAI SDK. It uses the same stable OpenAI/Microsoft.Extensions.AI abstractions and retains the existing retrieval and response contract.

The document-preparation CLI uses stable System.CommandLine 2.0.0. Central pins include System.Text.Json and FileSystemGlobbing 10.0.12, SixLabors.ImageSharp 2.1.13, and SixLabors.Fonts 1.0.0. The latter replaces an inherited prerelease dependency. Reference code uses stable C# 12, not preview language mode.

Build the complete reference web solution with:

```bash
dotnet build reference/rag-app/app/app.sln -c Release --source https://api.nuget.org/v3/index.json
```

The reference still targets .NET 8. Install its runtime/SDK before test execution or application startup. Optional MAUI workloads are separate prerequisites and are not provided by the workshop SDK alone.

The generated [stable Word edition](STUDENT-LAB-GUIDE-STABLE.docx) preserves the original guide's learning sequence and replaces obsolete inline snippets with checkpoint-source pointers. The original uploaded document is unchanged.