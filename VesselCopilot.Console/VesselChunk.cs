using System.Text.Json.Serialization;

public class VesselChunk
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("title")] public string Title { get; set; } = "";
    [JsonPropertyName("docType")] public string DocType { get; set; } = "";
    [JsonPropertyName("revision")] public string Revision { get; set; } = "";
    [JsonPropertyName("content")] public string Content { get; set; } = "";
    [JsonPropertyName("contentVector")] public float[]? ContentVector { get; set; }
}