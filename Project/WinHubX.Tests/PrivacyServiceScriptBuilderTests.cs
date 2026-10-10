using System.Diagnostics;
using Microsoft.Win32;
using System.Text;
using System.Runtime.Versioning;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

[SupportedOSPlatform("windows")]
public sealed class PrivacyServiceScriptBuilderTests
{
    [Fact]
    public async Task BuildEncodedCommand_ProducesValidPowerShellAndEncodesServiceData()
    {
        string encodedCommand = PrivacyServiceScriptBuilder.BuildEncodedCommand(
        [
            new PrivacyServiceChange("DiagTrack", "Disabled", false),
            new PrivacyServiceChange("dmwappushservice", "Automatic", true)
        ]);
        string script = Encoding.Unicode.GetString(Convert.FromBase64String(encodedCommand));
        Assert.DoesNotContain("DiagTrack", script, StringComparison.Ordinal);
        Assert.Contains("Stop-Service", script, StringComparison.Ordinal);
        Assert.Contains("Set-Service", script, StringComparison.Ordinal);
        Assert.Contains("Start-Service", script, StringComparison.Ordinal);

        await AssertValidPowerShellSyntaxAsync(encodedCommand);
    }

    [Fact]
    public async Task BuildEncodedCommand_WithRegistryBatch_ProducesValidCombinedScript()
    {
        ElevatedRegistryMutationBatch registryChanges = new();
        registryChanges.SetValue(RegistryHive.LocalMachine,
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
            "DisableDiagnostics", 1, RegistryValueKind.DWord, RegistryView.Registry64);
        string encodedCommand = PrivacyServiceScriptBuilder.BuildEncodedCommand(
            [new PrivacyServiceChange("DiagTrack", "Disabled", false)], registryChanges);
        string script = Encoding.Unicode.GetString(Convert.FromBase64String(encodedCommand));

        Assert.Contains("OpenBaseKey", script, StringComparison.Ordinal);
        Assert.Contains("Set-Service", script, StringComparison.Ordinal);
        await AssertValidPowerShellSyntaxAsync(encodedCommand);
    }

    private static async Task AssertValidPowerShellSyntaxAsync(string encodedCommand)
    {
        string parserScript = "$source = [Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('" + encodedCommand + "')); " +
            "$tokens = $null; $errors = $null; " +
            "[System.Management.Automation.Language.Parser]::ParseInput($source, [ref]$tokens, [ref]$errors) | Out-Null; " +
            "if ($errors.Count -gt 0) { $errors | ForEach-Object { [Console]::Error.WriteLine($_.Message) }; exit 1 }";
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(parserScript);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Impossibile avviare PowerShell per il test del parser.");
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string error = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        Assert.True(process.ExitCode == 0, error);
    }

    [Fact]
    public void BuildEncodedCommand_RejectsServiceNamesOutsidePrivacyAllowlist()
    {
        Assert.Throws<ArgumentException>(() => PrivacyServiceScriptBuilder.BuildEncodedCommand(
            [new PrivacyServiceChange("WinDefend", "Disabled", false)]));
    }

    [Fact]
    public void BuildEncodedCommand_RejectsStartupTypesOutsideWindowsServiceValues()
    {
        Assert.Throws<ArgumentException>(() => PrivacyServiceScriptBuilder.BuildEncodedCommand(
            [new PrivacyServiceChange("DiagTrack", "Boot", false)]));
    }
}
