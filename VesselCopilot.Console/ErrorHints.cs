using System.ClientModel;
using Azure;
using Azure.Identity;

public static class ErrorHints
{
    public static string Describe(Exception exception) => exception switch
    {
        OperationCanceledException => "Cancelled (Ctrl+C or the request timeout).",
        RequestFailedException request => $"Azure error {request.Status}: {Hint(request.Status)}",
        ClientResultException result => $"Model error {result.Status}: {Hint(result.Status)}",
        AuthenticationFailedException => "Authentication failed. Check az login and the selected tenant.",
        InvalidOperationException invalid => invalid.Message,
        _ => $"Unexpected error ({exception.GetType().Name})."
    };

    private static string Hint(int status) => status switch
    {
        400 => "check model support, request settings, or content filtering; do not blindly retry.",
        401 => "run az login again.",
        403 => "check the identity's data-plane role.",
        404 => "check endpoint and deployment or index name.",
        429 => "rate limited; reduce request volume and retry later.",
        _ => "check the service status and workshop troubleshooting guide."
    };
}