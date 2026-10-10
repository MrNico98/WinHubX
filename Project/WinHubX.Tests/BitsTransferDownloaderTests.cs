using System.Diagnostics;
using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class BitsTransferDownloaderTests
{
    [Fact]
    public void BuildTransferScript_UsesVerifiedHttpsAndFailClosedRedirects()
    {
        string encoded = BitsTransferDownloader.BuildTransferScript(
            new Uri("https://github.com/example/archive.zip"),
            Path.Combine(Path.GetTempPath(), "archive.zip"),
            "WinHubX-0123456789abcdef0123456789abcdef");
        string script = Encoding.Unicode.GetString(Convert.FromBase64String(encoded));

        Assert.Contains("Start-BitsTransfer", script, StringComparison.Ordinal);
        Assert.Contains("-SecurityFlags @('EnableCRLCheck', 'RedirectPolicyDisallow')", script, StringComparison.Ordinal);
        Assert.Contains("Complete-BitsTransfer", script, StringComparison.Ordinal);
        Assert.Contains("Remove-BitsTransfer", script, StringComparison.Ordinal);
        Assert.DoesNotContain("http://", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("https://github.com/example/archive.zip", script, StringComparison.Ordinal);
    }

    [Fact]
    public async Task BuildTransferScript_ParsesInWindowsPowerShell()
    {
        if (!OperatingSystem.IsWindows())
            return;

        string encoded = BitsTransferDownloader.BuildTransferScript(
            new Uri("https://github.com/example/archive.zip"),
            Path.Combine(Path.GetTempPath(), "archive.zip"),
            "WinHubX-0123456789abcdef0123456789abcdef");
        string parser = "$source = [Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('" + encoded + "')); " +
            "$tokens = $null; $errors = $null; " +
            "[System.Management.Automation.Language.Parser]::ParseInput($source, [ref]$tokens, [ref]$errors) | Out-Null; " +
            "if ($errors.Count -gt 0) { $errors | ForEach-Object { [Console]::Error.WriteLine($_.Message) }; exit 1 }";
        string parserEncoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(parser));
        ProcessStartInfo startInfo = new(Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"))
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(parserEncoded);

        using Process process = Process.Start(startInfo)!;
        string error = await process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        await process.WaitForExitAsync(TestContext.Current.CancellationToken);

        Assert.True(process.ExitCode == 0, error);
    }
}
