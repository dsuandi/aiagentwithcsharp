using MinimalApi.Services;
using Xunit;

namespace MinimalApi.Tests;

public class StableModelClientTests
{
    [Theory]
    [InlineData("https://approved.blob.core.windows.net/corpus/image.png", true)]
    [InlineData("http://approved.blob.core.windows.net/corpus/image.png", false)]
    [InlineData("https://untrusted.example/corpus/image.png", false)]
    [InlineData("https://approved.blob.core.windows.net/other/image.png", false)]
    [InlineData("https://approved.blob.core.windows.net/corpus/image.png?redirect=external", false)]
    public void ImageAuthorizationIsRestrictedToApprovedStorage(string imageUrl, bool expected)
    {
        Assert.Equal(expected, ReadRetrieveReadChatService.IsTrustedImageUri(new Uri(imageUrl),
            new Uri("https://approved.blob.core.windows.net/"), "corpus"));
    }
}