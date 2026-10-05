// Copyright (c) Microsoft. All rights reserved.

internal static partial class Program
{
    private static readonly Argument<string> s_files =
        new("files") { Description = "Files to be processed" };

    private static readonly Option<string> s_category =
        new("--category") { Description = "Value for the category field in the search index for all sections indexed in this run" };

    private static readonly Option<bool> s_skipBlobs =
        new("--skipblobs") { Description = "Skip uploading individual pages to Azure Blob Storage" };

    private static readonly Option<string> s_storageEndpoint =
        new("--storageendpoint") { Description = "Azure Blob Storage service endpoint" };

    private static readonly Option<string> s_container =
        new("--container") { Description = "Azure Blob Storage container name" };

    private static readonly Option<string> s_tenantId =
        new("--tenantid") { Description = "Optional. Use this to define the Azure directory where to authenticate" };

    private static readonly Option<string> s_searchService =
        new("--searchendpoint") { Description = "The Azure AI Search service endpoint where content should be indexed (must exist already)" };

    private static readonly Option<string> s_searchIndexName =
        new("--searchindex") { Description = "Name of the Azure AI Search index where content should be indexed (will be created if it doesn't exist)" };

    private static readonly Option<string> s_azureOpenAIService =
        new("--openaiendpoint") { Description = "Azure OpenAI service endpoint (must exist already)" };

    private static readonly Option<string> s_embeddingModelName =
        new("--embeddingmodel") { Description = "Embedding deployment name (must exist already)" };

    private static readonly Option<bool> s_remove =
    new("--remove") { Description = "Remove references to this document from blob storage and the search index" };

    private static readonly Option<bool> s_removeAll =
        new("--removeall") { Description = "Remove all blobs from blob storage and documents from the search index" };

    private static readonly Option<string> s_formRecognizerServiceEndpoint =
        new("--formrecognizerendpoint") { Description = "Azure AI Document Intelligence service endpoint (must exist already)" };

    private static readonly Option<string> s_computerVisionServiceEndpoint =
        new("--computervisionendpoint") { Description = "Azure Computer Vision service endpoint for image vectors" };

    private static readonly Option<bool> s_verbose =
    new("--verbose", "-v") { Description = "Verbose output" };

    private static readonly RootCommand s_rootCommand =
        new(description: """
        Prepare documents by extracting content from PDFs, splitting content into sections,
        uploading to blob storage, and indexing in a search index.
        """);

    static Program()
    {
        s_rootCommand.Arguments.Add(s_files);
        foreach (Option option in new Option[]
        {
            s_category,
            s_skipBlobs,
            s_storageEndpoint,
            s_container,
            s_tenantId,
            s_searchService,
            s_searchIndexName,
            s_azureOpenAIService,
            s_embeddingModelName,
            s_remove,
            s_removeAll,
            s_formRecognizerServiceEndpoint,
            s_computerVisionServiceEndpoint,
            s_verbose,
        }) s_rootCommand.Options.Add(option);
    }

        private static AppOptions GetParsedAppOptions(ParseResult context) => new(
            Files: context.GetValue(s_files) ?? "",
            Category: context.GetValue(s_category),
            SkipBlobs: context.GetValue(s_skipBlobs),
            StorageServiceBlobEndpoint: context.GetValue(s_storageEndpoint),
            Container: context.GetValue(s_container),
            TenantId: context.GetValue(s_tenantId),
            SearchServiceEndpoint: context.GetValue(s_searchService),
            SearchIndexName: context.GetValue(s_searchIndexName),
            AzureOpenAIServiceEndpoint: context.GetValue(s_azureOpenAIService),
            EmbeddingModelName: context.GetValue(s_embeddingModelName),
            Remove: context.GetValue(s_remove),
            RemoveAll: context.GetValue(s_removeAll),
            FormRecognizerServiceEndpoint: context.GetValue(s_formRecognizerServiceEndpoint),
            ComputerVisionServiceEndpoint: context.GetValue(s_computerVisionServiceEndpoint),
            Verbose: context.GetValue(s_verbose),
            Console: Console.Out);
}
