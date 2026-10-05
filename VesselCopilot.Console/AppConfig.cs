using Microsoft.Extensions.Configuration;

public static class AppConfig
{
    private static IConfiguration? _config;
    public static IConfiguration Config =>
        _config ?? throw new InvalidOperationException("Call AppConfig.Load(args) first.");
    public static FoundrySettings Foundry { get; private set; } = new();
    public static AuthSettings Auth { get; private set; } = new();
    public static string ActiveProfile { get; private set; } = "";
    public static ModelProfile Model { get; private set; } = new();
    public static EmbeddingSettings Embedding { get; private set; } = new();
    public static SearchSettings Search { get; private set; } = new();

    public static void Load(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets(typeof(AppConfig).Assembly, optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        Foundry = Config.GetSection("Foundry").Get<FoundrySettings>()
            ?? throw new InvalidOperationException("appsettings.json has no Foundry section.");
        RequireHttpsUri("Foundry:ProjectEndpoint", Foundry.ProjectEndpoint);
        Auth = Config.GetSection("Auth").Get<AuthSettings>() ?? new();
        ActiveProfile = Require("Model:ActiveProfile", Config["Model:ActiveProfile"]);
        Model = GetProfile(ActiveProfile);
        Embedding = Config.GetSection("Embedding").Get<EmbeddingSettings>() ?? new();
        Search = Config.GetSection("Search").Get<SearchSettings>() ?? new();
    }

    public static ModelProfile GetProfile(string name)
    {
        ModelProfile profile = Config.GetSection($"Model:Profiles:{name}").Get<ModelProfile>()
            ?? throw new InvalidOperationException($"Model profile '{name}' was not found under Model:Profiles.");
        Require($"Model:Profiles:{name}:DeploymentName", profile.DeploymentName);
        if (profile.MaxOutputTokens is < 1 or > 4000)
            throw new InvalidOperationException($"Model:Profiles:{name}:MaxOutputTokens must be between 1 and 4000.");
        return profile;
    }

    public static string Require(string key, string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Contains('<')
            ? throw new InvalidOperationException($"Setting '{key}' is missing or still a placeholder. Edit appsettings.json.")
            : value;

    public static Uri RequireHttpsUri(string key, string? value) =>
        Uri.TryCreate(Require(key, value), UriKind.Absolute, out Uri? uri) && uri.Scheme == Uri.UriSchemeHttps
            ? uri
            : throw new InvalidOperationException($"Setting '{key}' must be an absolute https:// URL.");

    public static string Describe() =>
        $"Using profile {ActiveProfile} -> deployment {Model.DeploymentName} " +
        $"(model {Model.ModelName}, version {Model.ModelVersion})";
}