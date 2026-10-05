using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

public static partial class Ingest
{
    [GeneratedRegex("[^A-Za-z0-9_=-]")]
    private static partial Regex InvalidKeyChars();

    public static string ReadMetadata(string text, string key) =>
        text.Split('\n').Select(line => line.Trim())
            .FirstOrDefault(line => line.StartsWith(key + ":", StringComparison.Ordinal))?[(key.Length + 1)..].Trim() ?? "";

    public static async Task<List<VesselChunk>> ReadDocumentsAsync(CancellationToken cancellationToken)
    {
        var documents = new List<VesselChunk>();
        string folder = Path.Combine(AppContext.BaseDirectory, "data");
        foreach (string file in Directory.GetFiles(folder, "*.md").Order(StringComparer.Ordinal))
        {
            string text = await File.ReadAllTextAsync(file, cancellationToken);
            string fileKey = Path.GetFileNameWithoutExtension(file);
            List<string> chunks = TextChunker.Chunk(text);
            for (int index = 0; index < chunks.Count; index++)
                documents.Add(new VesselChunk
                {
                    Id = InvalidKeyChars().Replace($"{fileKey}-{index}", "_"),
                    Title = ReadMetadata(text, "Title"),
                    DocType = ReadMetadata(text, "Type"),
                    Revision = ReadMetadata(text, "Revision"),
                    Content = chunks[index]
                });
            Console.WriteLine($"{fileKey}: {chunks.Count} chunk(s)");
        }
        if (documents.Count == 0) throw new InvalidOperationException("No synthetic documents found in the data folder.");
        return documents;
    }

    public static async Task RunAsync(CancellationToken cancellationToken)
    {
        List<VesselChunk> documents = await ReadDocumentsAsync(cancellationToken);
        foreach (VesselChunk[] batch in documents.Chunk(16))
        {
            GeneratedEmbeddings<Embedding<float>> vectors = await EmbeddingFactory.Shared.GenerateAsync(
                batch.Select(document => document.Content), EmbeddingFactory.Options, cancellationToken);
            if (vectors.Count != batch.Length) throw new InvalidOperationException("Embedding batch count mismatch.");
            for (int index = 0; index < batch.Length; index++)
            {
                if (vectors[index].Vector.Length != AppConfig.Embedding.Dimensions)
                    throw new InvalidOperationException("Embedding dimension mismatch; no documents were uploaded.");
                batch[index].ContentVector = vectors[index].Vector.ToArray();
            }
        }
        await SearchSetup.CreateIndexAsync(cancellationToken);
        foreach (VesselChunk[] batch in documents.Chunk(500))
        {
            var result = await SearchSetup.SharedSearchClient.MergeOrUploadDocumentsAsync(batch,
                cancellationToken: cancellationToken);
            if (result.Value.Results.Any(item => !item.Succeeded))
                throw new InvalidOperationException("One or more Search documents failed to upload.");
        }
        Console.WriteLine($"Uploaded {documents.Count} chunks to '{SearchSetup.IndexName}'.");
    }
}