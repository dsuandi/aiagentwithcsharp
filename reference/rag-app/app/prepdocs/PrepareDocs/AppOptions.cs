// Copyright (c) Microsoft. All rights reserved.

namespace PrepareDocs;

internal record class AppOptions(
    string Files,
    string? Category,
    bool SkipBlobs,
    string? StorageServiceBlobEndpoint,
    string? Container,
    string? TenantId,
    string? SearchServiceEndpoint,
    string? AzureOpenAIServiceEndpoint,
    string? SearchIndexName,
    string? EmbeddingModelName,
    bool Remove,
    bool RemoveAll,
    string? FormRecognizerServiceEndpoint,
    string? ComputerVisionServiceEndpoint,
    bool Verbose,
    TextWriter Console) : AppConsole(Console);

internal record class AppConsole(TextWriter Console);
