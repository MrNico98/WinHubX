using System.Net;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class TrustedHttpsClientTests
{
    [Theory]
    [InlineData("https://raw.githubusercontent.com/owner/repo/main/file.json")]
    [InlineData("https://github.com/owner/repo/releases/download/v1/app.exe")]
    [InlineData("https://officecdn.microsoft.com/path/package.cab")]
    [InlineData("https://c2rsetup.officeapps.live.com/path/setup.exe")]
    [InlineData("https://aka.ms/getwinget")]
    public void ValidateUri_AcceptsAllowlistedHttpsHosts(string address)
    {
        Uri result = TrustedHttpsClient.ValidateUri(address, "test");

        Assert.Equal(Uri.UriSchemeHttps, result.Scheme);
    }

    [Theory]
    [InlineData("https://github.com.evil.example/owner/repo/file")]
    [InlineData("https://user@github.com/owner/repo/file")]
    [InlineData("https://github.com:8443/owner/repo/file")]
    [InlineData("https://example.org/file")]
    [InlineData("not a url")]
    public void ValidateUri_RejectsUntrustedAddresses(string address)
    {
        Assert.Throws<InvalidDataException>(() => TrustedHttpsClient.ValidateUri(address, "test"));
    }

    [Fact]
    public void ValidateUri_RejectsHttpEvenWhenTheHostIsAllowlisted()
    {
        string address = new UriBuilder(Uri.UriSchemeHttp, "raw.githubusercontent.com")
        {
            Path = "owner/repo/file"
        }.Uri.AbsoluteUri;

        Assert.Throws<InvalidDataException>(() => TrustedHttpsClient.ValidateUri(address, "test"));
    }

    [Fact]
    public async Task SendAsync_FollowsOnlyAllowlistedHttpsRedirects()
    {
        int requests = 0;
        using var handler = new StubHandler(request =>
        {
            requests++;
            return requests == 1
                ? Redirect("https://release-assets.githubusercontent.com/file.exe")
                : new HttpResponseMessage(HttpStatusCode.OK);
        });
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/owner/repo/file.exe");

        using HttpResponseMessage response = await TrustedHttpsClient.SendAsync(
            client,
            request,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, requests);
    }

    [Theory]
    [InlineData("https://evil.example/file.exe")]
    public async Task SendAsync_RejectsUnsafeRedirectBeforeRequestingDestination(string destination)
    {
        int requests = 0;
        using var handler = new StubHandler(_ =>
        {
            requests++;
            return Redirect(destination);
        });
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/owner/repo/file.exe");

        await Assert.ThrowsAsync<InvalidDataException>(() => TrustedHttpsClient.SendAsync(
            client,
            request,
            TestContext.Current.CancellationToken));

        Assert.Equal(1, requests);
    }

    [Fact]
    public async Task SendAsync_RejectsHttpRedirectBeforeRequestingDestination()
    {
        int requests = 0;
        string destination = new UriBuilder(Uri.UriSchemeHttp, "raw.githubusercontent.com")
        {
            Path = "file.exe"
        }.Uri.AbsoluteUri;
        using var handler = new StubHandler(_ =>
        {
            requests++;
            return Redirect(destination);
        });
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/owner/repo/file.exe");

        await Assert.ThrowsAsync<InvalidDataException>(() => TrustedHttpsClient.SendAsync(
            client,
            request,
            TestContext.Current.CancellationToken));

        Assert.Equal(1, requests);
    }

    [Fact]
    public async Task SendAsync_RejectsRedirectChainsLongerThanMaximum()
    {
        int requests = 0;
        using var handler = new StubHandler(_ =>
        {
            requests++;
            var redirectUri = new UriBuilder(Uri.UriSchemeHttps, "github.com")
            {
                Path = $"/owner/repo/redirect-{requests}"
            };
            return Redirect(redirectUri.Uri.AbsoluteUri);
        });
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://github.com/owner/repo/start");

        await Assert.ThrowsAsync<HttpRequestException>(() => TrustedHttpsClient.SendAsync(
            client,
            request,
            TestContext.Current.CancellationToken));

        Assert.Equal(6, requests);
    }

    [Fact]
    public async Task GetStringAsync_AppliesClientTimeoutToResponseBody()
    {
        using var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new DelayedContent()
        });
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(50) };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => TrustedHttpsClient.GetStringAsync(
            client,
            "https://raw.githubusercontent.com/owner/repo/main/file.json",
            TestContext.Current.CancellationToken));
    }

    private static HttpResponseMessage Redirect(string destination) => new(HttpStatusCode.Redirect)
    {
        Headers = { Location = new Uri(destination) }
    };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> createResponse) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(createResponse(request));
    }

    private sealed class DelayedContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            Task.Delay(Timeout.InfiniteTimeSpan);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken) =>
            Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
