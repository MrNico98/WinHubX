using Xunit;

namespace WinHubX.Tests;

public sealed class ThemeConfigTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(), "WinHubX.Tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void SaveAndLoad_PreserveThemeAndLanguagePreferences()
    {
        string settingsPath = Path.Combine(_testDirectory, "nested", "Tema.json");
        var expected = new ThemeConfig
        {
            DarkTheme = true,
            ThemeManuallySet = true,
            Language = "it",
            LanguageManuallySet = true
        };

        expected.Save(settingsPath);
        ThemeConfig actual = ThemeConfig.Load(settingsPath);

        Assert.True(actual.DarkTheme);
        Assert.True(actual.ThemeManuallySet);
        Assert.Equal("it", actual.Language);
        Assert.True(actual.LanguageManuallySet);
        Assert.Empty(Directory.GetFiles(Path.GetDirectoryName(settingsPath)!, "*.tmp"));
    }

    [Fact]
    public void Load_MissingFileCreatesDefaults()
    {
        string settingsPath = Path.Combine(_testDirectory, "Tema.json");

        ThemeConfig actual = ThemeConfig.Load(settingsPath);

        Assert.True(File.Exists(settingsPath));
        Assert.False(actual.DarkTheme);
        Assert.False(actual.ThemeManuallySet);
        Assert.Equal("en", actual.Language);
        Assert.False(actual.LanguageManuallySet);
    }

    [Fact]
    public void Load_InvalidJsonReturnsDefaultsAndPreservesOriginalFile()
    {
        string settingsPath = Path.Combine(_testDirectory, "Tema.json");
        const string invalidJson = "{ invalid";
        Directory.CreateDirectory(_testDirectory);
        File.WriteAllText(settingsPath, invalidJson);

        ThemeConfig actual = ThemeConfig.Load(settingsPath);

        Assert.False(actual.DarkTheme);
        Assert.Equal("en", actual.Language);
        Assert.Equal(invalidJson, File.ReadAllText(settingsPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }
}
