using Azure.Core;
using Azure.Identity;

public static class CredentialFactory
{
    private static TokenCredential? _credential;
    public static TokenCredential Credential => _credential ??= AppConfig.Auth.UseManagedIdentity
        ? new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned)
        : new DefaultAzureCredential();
}