namespace WinHubX.Impostazioni;

public static class TrustedHttpsClient
{
    private const int MaximumRedirects = 5;
    private static readonly HashSet<string> TrustedHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "aka.ms",
        "c2rsetup.officeapps.live.com",
        "catalog.update.microsoft.com",
        "dl.delivery.mp.microsoft.com",
        "download.microsoft.com",
        "download.windowsupdate.com",
        "github.com",
        "go.microsoft.com",
        "officecdn.microsoft.com",
        "objects.githubusercontent.com",
        "raw.githubusercontent.com",
        "release-assets.githubusercontent.com",
        "software-download.microsoft.com",
        "www.microsoft.com"
    };

    public static Uri ValidateUri(string? value, string purpose)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
            || !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || uri.Port != 443
            || uri.UserInfo.Length != 0
            || !IsTrustedHost(uri.Host))
        {
            throw new InvalidDataException($"URL HTTPS non attendibile per {purpose}.");
        }

        return uri;
    }

    public static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(request);
        if (request.Content is not null)
        {
            throw new NotSupportedException("Il client HTTPS attendibile accetta solo richieste senza body.");
        }

        Uri currentUri = ValidateUri(request.RequestUri?.AbsoluteUri, "richiesta HTTP");
        for (int redirectCount = 0; redirectCount <= MaximumRedirects; redirectCount++)
        {
            using var safeRequest = new HttpRequestMessage(request.Method, currentUri)
            {
                Version = request.Version,
                VersionPolicy = request.VersionPolicy
            };
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                _ = safeRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            HttpResponseMessage response = await client.SendAsync(
                safeRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!IsRedirect(response.StatusCode))
            {
                return response;
            }

            Uri? location = response.Headers.Location;
            response.Dispose();
            if (location is null)
            {
                throw new HttpRequestException("Il server ha restituito un redirect senza destinazione.");
            }
            if (redirectCount == MaximumRedirects)
            {
                throw new HttpRequestException("Numero massimo di redirect superato.");
            }

            string locationValue = location.IsAbsoluteUri ? location.AbsoluteUri : new Uri(currentUri, location).AbsoluteUri;
            currentUri = ValidateUri(locationValue, "redirect HTTP");
        }

        throw new InvalidOperationException("Il ciclo di redirect è terminato in modo inatteso.");
    }

    public static async Task<HttpResponseMessage> GetAsync(
        HttpClient client,
        string url,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ValidateUri(url, "download"));
        return await SendAsync(client, request, cancellationToken);
    }

    public static async Task<string> GetStringAsync(
        HttpClient client,
        string url,
        CancellationToken cancellationToken = default)
    {
        using var timeout = new CancellationTokenSource(client.Timeout);
        using var operationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        using HttpResponseMessage response = await GetAsync(client, url, operationToken.Token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(operationToken.Token);
    }

    internal static async Task<Uri> ResolveDownloadUriAsync(
        HttpClient client,
        string url,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await GetAsync(client, url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return ValidateUri(response.RequestMessage?.RequestUri?.AbsoluteUri, "download BITS");
    }

    private static bool IsTrustedHost(string host) =>
        TrustedHosts.Contains(host)
        || host.EndsWith(".githubusercontent.com", StringComparison.OrdinalIgnoreCase);

    private static bool IsRedirect(System.Net.HttpStatusCode statusCode) =>
        statusCode is System.Net.HttpStatusCode.MovedPermanently
            or System.Net.HttpStatusCode.Redirect
            or System.Net.HttpStatusCode.RedirectMethod
            or System.Net.HttpStatusCode.TemporaryRedirect
            or System.Net.HttpStatusCode.PermanentRedirect;
}
