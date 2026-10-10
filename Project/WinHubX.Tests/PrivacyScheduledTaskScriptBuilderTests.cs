using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

[SupportedOSPlatform("windows")]
public sealed class PrivacyScheduledTaskScriptBuilderTests
{
    [Fact]
    public async Task BuildScript_EncodesTaskNamesAndProducesValidPowerShell()
    {
        string script = PrivacyScheduledTaskScriptBuilder.BuildScript(
        [
            new PrivacyScheduledTaskChange(@"Microsoft\Windows\Windows Error Reporting\QueueReporting", Enable: true),
            new PrivacyScheduledTaskChange(@"Microsoft\Windows\Defrag\ScheduledDefrag", Enable: false)
        ]);

        Assert.DoesNotContain("QueueReporting", script, StringComparison.Ordinal);
        Assert.Contains("Get-ScheduledTask", script, StringComparison.Ordinal);
        Assert.Contains("Enable-ScheduledTask", script, StringComparison.Ordinal);
        Assert.Contains("Disable-ScheduledTask", script, StringComparison.Ordinal);
        await AssertValidPowerShellSyntaxAsync(script);
    }

    [Fact]
    public void BuildScript_RejectsTasksOutsideAllowlist()
    {
        Assert.Throws<ArgumentException>(() => PrivacyScheduledTaskScriptBuilder.BuildScript(
            [new PrivacyScheduledTaskChange(@"Microsoft\Windows\Defender\ScheduledScan", Enable: false)]));
    }

    [Fact]
    public void BuildScript_RejectsEmptyTaskList()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PrivacyScheduledTaskScriptBuilder.BuildScript([]));
    }

    private static async Task AssertValidPowerShellSyntaxAsync(string script)
    {
        string encodedScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        string parserCommand = "$script=[Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('" + encodedScript + "')); " +
            "$tokens=$null; $errors=$null; [System.Management.Automation.Language.Parser]::ParseInput($script, [ref]$tokens, [ref]$errors) > $null; " +
            "if ($errors.Count -gt 0) { $errors | ForEach-Object { [Console]::Error.WriteLine($_.Message) }; exit 1 }";
        ProcessStartInfo startInfo = new(Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"))
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(parserCommand);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Impossibile avviare PowerShell per il test del parser.");
        string error = await process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        await process.WaitForExitAsync(TestContext.Current.CancellationToken);
        Assert.True(process.ExitCode == 0, error);
    }
}
