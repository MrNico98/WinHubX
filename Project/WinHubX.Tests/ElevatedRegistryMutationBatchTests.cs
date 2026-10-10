using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

[SupportedOSPlatform("windows")]
public sealed class ElevatedRegistryMutationBatchTests
{
    [Fact]
    public void BuildCommand_ContainsSerializedTargetAndTypedValue()
    {
        ElevatedRegistryMutationBatch batch = new();
        batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\WinHubX", "Enabled", 1,
            RegistryValueKind.DWord, RegistryView.Registry64);

        string script = batch.BuildCommand();
        string serializedMutations = script.Split("$payload = '", StringSplitOptions.None)[1].Split('\'')[0];
        string json = Encoding.UTF8.GetString(Convert.FromBase64String(serializedMutations));

        Assert.Contains("ConvertFrom-Json", script, StringComparison.Ordinal);
        Assert.Contains("RegistryValueKind", script, StringComparison.Ordinal);
        Assert.Contains("OpenBaseKey", script, StringComparison.Ordinal);
        Assert.Contains("DeleteValue", script, StringComparison.Ordinal);
        Assert.Contains("SOFTWARE\\\\Policies\\\\WinHubX", json, StringComparison.Ordinal);
        Assert.Contains("DWord", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SetValue_PreservesHighBitInUnsignedDword()
    {
        ElevatedRegistryMutationBatch batch = new();
        batch.SetValue(RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators",
            2147483648U, RegistryValueKind.DWord, RegistryView.Registry64);

        string script = batch.BuildCommand();
        string serializedMutations = script.Split("$payload = '", StringSplitOptions.None)[1].Split('\'')[0];
        string json = Encoding.UTF8.GetString(Convert.FromBase64String(serializedMutations));

        Assert.Contains("2147483648", json, StringComparison.Ordinal);
        Assert.Contains("ToInt32", script, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildCommand_IsValidPowerShellSyntax()
    {
        ElevatedRegistryMutationBatch batch = new();
        batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\WinHubX", "Enabled", 1,
            RegistryValueKind.DWord, RegistryView.Registry64);

        AssertValidPowerShellSyntax(batch.BuildCommand());
    }

    [Fact]
    public void SetValue_SerializesBinaryValuesAsBase64AndProducesValidPowerShell()
    {
        ElevatedRegistryMutationBatch batch = new();
        byte[] expectedValue = [0, 0, 0, 0];
        batch.SetValue(RegistryHive.LocalMachine, @"SYSTEM\GameConfigStore", "GameDVR_Enabled",
            expectedValue, RegistryValueKind.Binary, RegistryView.Registry64);

        string script = batch.BuildCommand();
        string serializedMutations = script.Split("$payload = '", StringSplitOptions.None)[1].Split('\'')[0];
        string json = Encoding.UTF8.GetString(Convert.FromBase64String(serializedMutations));

        Assert.Contains(Convert.ToBase64String(expectedValue), json, StringComparison.Ordinal);
        Assert.Contains("Binary", json, StringComparison.Ordinal);
        Assert.Contains("FromBase64String", script, StringComparison.Ordinal);
        AssertValidPowerShellSyntax(script);
    }

    [Fact]
    public void BuildCommand_WithElevatedDismTail_IsValidPowerShellSyntax()
    {
        ElevatedRegistryMutationBatch batch = new();
        batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot",
            "TurnOffWindowsCopilot", 1, RegistryValueKind.DWord, RegistryView.Registry32);
        string script = string.Join(Environment.NewLine,
            batch.BuildCommand(),
            "$dism = Join-Path $env:SystemRoot 'System32\\dism.exe'",
            "& $dism /online /remove-package /package-name:Microsoft.Windows.Copilot",
            "exit $LASTEXITCODE");

        AssertValidPowerShellSyntax(script);
    }

    private static void AssertValidPowerShellSyntax(string script)
    {
        string encodedScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        string parserCommand = "$encoded='" + encodedScript + "'; $script=[Text.Encoding]::Unicode.GetString([Convert]::FromBase64String($encoded)); $errors=$null; [System.Management.Automation.Language.Parser]::ParseInput($script, [ref]$null, [ref]$errors) > $null; if ($errors.Count -gt 0) { $errors | ForEach-Object { Write-Error $_.Message }; exit 1 }";
        string powershellPath = Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe");
        ProcessStartInfo startInfo = new(powershellPath)
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(parserCommand);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Impossibile avviare PowerShell per validare la sintassi.");
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Assert.True(process.ExitCode == 0, standardError);
    }

    [Fact]
    public void ElevatedRegistryAclMutationBatch_ProducesValidRestrictedPowerShell()
    {
        ElevatedRegistryAclMutationBatch batch = new();
        batch.AddLocalMachineTakeOwnership(
            @"SYSTEM\CurrentControlSet\Services\WinDefend",
            RegistryView.Registry64,
            "S-1-5-21-111111111-222222222-333333333-1001");

        string script = batch.BuildCommand();

        Assert.Contains("RegistryRights]::TakeOwnership", script, StringComparison.Ordinal);
        Assert.Contains("OpenBaseKey", script, StringComparison.Ordinal);
        AssertValidPowerShellSyntax(script);
    }

    [Fact]
    public void ElevatedRegistryAclMutationBatch_RejectsKeysOutsideDefenderServices()
    {
        ElevatedRegistryAclMutationBatch batch = new();

        Assert.Throws<ArgumentException>(() => batch.AddLocalMachineTakeOwnership(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            RegistryView.Registry64,
            "S-1-5-18"));
    }

    [Theory]
    [InlineData(RegistryHive.CurrentUser)]
    [InlineData(RegistryHive.ClassesRoot)]
    public void SetValue_RejectsHivesOutsideSystemScope(RegistryHive hive)
    {
        ElevatedRegistryMutationBatch batch = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => batch.SetValue(hive, @"Software\WinHubX", "Enabled", 1,
            RegistryValueKind.DWord, RegistryView.Default));
    }

    [Theory]
    [InlineData(@"..\Software\WinHubX")]
    [InlineData(@"Software\\WinHubX")]
    [InlineData(@"\Software\WinHubX")]
    public void SetValue_RejectsInvalidSubKeyPath(string path)
    {
        ElevatedRegistryMutationBatch batch = new();

        Assert.Throws<ArgumentException>(() => batch.SetValue(RegistryHive.LocalMachine, path, "Enabled", 1,
            RegistryValueKind.DWord, RegistryView.Default));
    }

    [Fact]
    public void BuildCommand_RejectsEmptyBatch()
    {
        Assert.Throws<InvalidOperationException>(() => new ElevatedRegistryMutationBatch().BuildCommand());
    }
}
