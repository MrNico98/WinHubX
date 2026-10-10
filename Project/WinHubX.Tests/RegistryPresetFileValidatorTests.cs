using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class RegistryPresetFileValidatorTests : IDisposable
{
    private const string ValidHeader = "Windows Registry Editor Version 5.00";
    private readonly string _temporaryDirectory = Path.Combine(Path.GetTempPath(), $"WinHubX.Tests-{Guid.NewGuid():N}");

    public RegistryPresetFileValidatorTests()
    {
        Directory.CreateDirectory(_temporaryDirectory);
    }

    [Fact]
    public void Validate_AllowsWinHubXRootAndChildKeysInUtf16Export()
    {
        string filePath = WritePreset(
            $"{ValidHeader}\r\n\r\n[HKEY_CURRENT_USER\\Software\\WinHubX]\r\n\"Theme\"=dword:00000001\r\n\r\n[HKEY_CURRENT_USER\\Software\\WinHubX\\Personalizzazione]\r\n\"DarkMode\"=dword:00000001\r\n",
            Encoding.Unicode);

        RegistryPresetFileValidator.Validate(filePath);
    }

    [Fact]
    public void Validate_AllowsLegacyRegedit4Header()
    {
        string filePath = WritePreset(
            "REGEDIT4\r\n\r\n[HKEY_CURRENT_USER\\Software\\WinHubX]\r\n\"Theme\"=dword:00000001\r\n",
            Encoding.ASCII);

        RegistryPresetFileValidator.Validate(filePath);
    }

    [Theory]
    [InlineData("[HKEY_LOCAL_MACHINE\\Software\\WinHubX]")]
    [InlineData("[HKEY_CURRENT_USER\\Software\\WinHubX-Evil]")]
    [InlineData("[-HKEY_CURRENT_USER\\Software\\WinHubX]")]
    [InlineData("[HKEY_CURRENT_USER\\Software\\WinHubX")]
    public void Validate_RejectsOutOfScopeOrMalformedRegistrySections(string section)
    {
        string filePath = WritePreset($"{ValidHeader}\r\n\r\n{section}\r\n\"Value\"=dword:00000001\r\n");

        Assert.Throws<InvalidDataException>(() => RegistryPresetFileValidator.Validate(filePath));
    }

    [Fact]
    public void Validate_RejectsUnsupportedHeader()
    {
        string filePath = WritePreset("Not a registry export\r\n[HKEY_CURRENT_USER\\Software\\WinHubX]\r\n");

        Assert.Throws<InvalidDataException>(() => RegistryPresetFileValidator.Validate(filePath));
    }

    [Fact]
    public void Validate_RejectsExportWithoutWinHubXKey()
    {
        string filePath = WritePreset($"{ValidHeader}\r\n\r\n; no keys\r\n");

        Assert.Throws<InvalidDataException>(() => RegistryPresetFileValidator.Validate(filePath));
    }

    [Fact]
    public void Validate_RejectsFilesOverOneMiB()
    {
        string filePath = Path.Combine(_temporaryDirectory, "oversized.reg");
        File.WriteAllBytes(filePath, new byte[(1024 * 1024) + 1]);

        Assert.Throws<InvalidDataException>(() => RegistryPresetFileValidator.Validate(filePath));
    }

    private string WritePreset(string contents, Encoding? encoding = null)
    {
        string filePath = Path.Combine(_temporaryDirectory, $"{Guid.NewGuid():N}.reg");
        File.WriteAllText(filePath, contents, encoding ?? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return filePath;
    }

    public void Dispose()
    {
        Directory.Delete(_temporaryDirectory, recursive: true);
    }
}
