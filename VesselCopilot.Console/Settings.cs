public sealed class FoundrySettings
{
    public string ProjectEndpoint { get; set; } = "";
    public string OpenAIEndpoint { get; set; } = "";
    public string ApiVersion { get; set; } = "";
}

public sealed class AuthSettings
{
    public bool UseManagedIdentity { get; set; }
}

public sealed class ModelProfile
{
    public string DeploymentName { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string ModelVersion { get; set; } = "";
    public int MaxOutputTokens { get; set; } = 800;
}

public sealed class EmbeddingSettings
{
    public string DeploymentName { get; set; } = "";
    public int Dimensions { get; set; }
}

public sealed class SearchSettings
{
    public string Endpoint { get; set; } = "";
    public string IndexName { get; set; } = "";
}