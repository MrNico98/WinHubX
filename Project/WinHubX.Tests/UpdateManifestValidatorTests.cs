using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class UpdateManifestValidatorTests
{
    private const string ValidUrl = "https://github.com/LightYagami28/WinHubX/releases/download/v1.2.3/WinHubX.exe";
    private const string ValidSha256 = "1806bb59260e230f2dd3f4817c78d5adc1a2a87375eded4ae93c8511c02d2ce0";

    [Fact]
    public void Validate_AcceptsReleaseWithSha256()
    {
        ValidatedUpdateManifest result = UpdateManifestValidator.Validate("1.2.3", ValidUrl, ValidSha256);

        Assert.Equal("1.2.3", result.Version);
        Assert.Equal(ValidUrl, result.UpdateUrl);
        Assert.Equal(ValidSha256, result.Sha256);
    }

    [Theory]
    [InlineData(null, ValidUrl, ValidSha256)]
    [InlineData("not-a-version", ValidUrl, ValidSha256)]
    [InlineData("1.2.3", "https://github.com.evil.example/owner/repo/releases/download/v1/file.exe", ValidSha256)]
    [InlineData("1.2.3", "https://user@github.com/owner/repo/releases/download/v1/file.exe", ValidSha256)]
    [InlineData("1.2.3", "https://github.com/owner/repo/releases/tag/v1/file.exe", ValidSha256)]
    [InlineData("1.2.3", ValidUrl, null)]
    [InlineData("1.2.3", ValidUrl, "not-a-digest")]
    public void Validate_RejectsInvalidManifest(string? version, string? url, string? sha256)
    {
        Assert.Throws<InvalidDataException>(() => UpdateManifestValidator.Validate(version, url, sha256));
    }

    [Fact]
    public void Validate_RejectsHttpReleaseUrl()
    {
        string url = new UriBuilder(Uri.UriSchemeHttp, "github.com")
        {
            Path = "owner/repo/releases/download/v1/file.exe"
        }.Uri.AbsoluteUri;

        Assert.Throws<InvalidDataException>(() => UpdateManifestValidator.Validate("1.2.3", url, ValidSha256));
    }
}
