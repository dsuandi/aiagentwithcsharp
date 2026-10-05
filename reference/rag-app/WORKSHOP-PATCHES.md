# Reference Source Provenance

Upstream: https://github.com/Azure-Samples/azure-search-openai-demo-csharp

Baseline commit: `7d702a68d37f8c93d902acf8a35088979a2e3fa3`.

This directory contains the upstream source snapshot with its original license and notices intact, plus the following workshop corrections. The latest stable edition supersedes the initial dependency-only remediation:

- Remove the old Semantic Kernel connector dependency, whose updated Azure connector still requires a beta SDK. Use stable OpenAI and Microsoft.Extensions.AI calls for the same retrieval flow and response contract.
- Pin Azure.AI.OpenAI 2.1.0, Azure.Identity 1.21.0, Microsoft.Extensions.AI.OpenAI 10.10.1, System.Text.Json/FileSystemGlobbing 10.0.12, SixLabors.ImageSharp 2.1.13, and stable SixLabors.Fonts 1.0.0.
- Migrate the ingestion CLI to stable System.CommandLine 2.0.0 and use stable C# 12.
- Restrict authenticated source-image downloads to the approved Storage account/container with redirects disabled. Generate follow-up questions as a validated object-shaped typed schema.
- Keep the original net8.0 target and original infrastructure. Cloud provisioning and full RAG runtime validation are blocked by the existing-resources-only constraint.

The unmodified reference graph reported critical/high vulnerabilities in Semantic Kernel Core 1.17.2, ImageSharp 1.0.4, and System.Text.Json 8.0.4. The stable web solution builds, the resolved core graph contains zero prerelease packages, and its audit reports no known vulnerable packages. Test execution still requires a .NET 8 runtime; compile success is not a test-pass claim.

This snapshot is included directly in Git, not as a submodule. Students need no unpublished upstream commit or extra submodule initialization. Refer to the workshop's review report for runtime and test limitations.