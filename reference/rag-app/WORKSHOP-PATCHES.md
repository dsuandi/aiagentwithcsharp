# Reference Source Provenance

Upstream: https://github.com/Azure-Samples/azure-search-openai-demo-csharp

Baseline commit: `7d702a68d37f8c93d902acf8a35088979a2e3fa3`.

This directory contains the upstream source snapshot with its original license and notices intact, plus the following focused workshop corrections:

- Enable central transitive pinning and pin Microsoft.SemanticKernel.Core 1.71.0, SixLabors.ImageSharp 2.1.13, and System.Text.Json 10.0.2.
- Preserve the existing Semantic Kernel connector integration. The legacy embedding-service interface is still supported but deprecated; one call site explicitly opts into that compatibility. This is not a wholesale framework modernization.
- Keep the original net8.0 target and original infrastructure. Cloud provisioning and full RAG runtime validation are blocked by the existing-resources-only constraint.

The unmodified reference graph reported critical/high vulnerabilities in Semantic Kernel Core 1.17.2, ImageSharp 1.0.4, and System.Text.Json 8.0.4. The corrected core backend/frontend build passed, and the resolved core graph reported no known vulnerable packages on 2026-10-06.

This snapshot is included directly in Git, not as a submodule. Students need no unpublished upstream commit or extra submodule initialization. Refer to the workshop's review report for runtime and test limitations.