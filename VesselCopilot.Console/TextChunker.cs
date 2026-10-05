public static class TextChunker
{
    public static List<string> Chunk(string text, int maxChars = 1000, int overlapChars = 150)
    {
        if (maxChars < 1 || overlapChars < 0 || overlapChars >= maxChars)
            throw new InvalidOperationException("Chunk size must be positive and overlap must be smaller than chunk size.");
        string normalized = text.ReplaceLineEndings("\n").Trim();
        var chunks = new List<string>();
        int start = 0;
        while (start < normalized.Length)
        {
            int length = Math.Min(maxChars, normalized.Length - start);
            string chunk = normalized.Substring(start, length).Trim();
            if (chunk.Length > 0) chunks.Add(chunk);
            if (start + length >= normalized.Length) break;
            start += length - overlapChars;
        }
        return chunks;
    }
}