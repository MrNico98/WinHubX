using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;
using WinHubX.Impostazioni;
using Xunit;
using Xunit.v3;

namespace WinHubX.Tests;

public sealed class ServiceConfigurationScriptBuilderTests
{
    [Fact]
    public void BuildEncodedCommand_EncodesServiceDataSeparatelyFromPowerShell()
    {
        string reportPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WinHubX service report.json"));
        string command = ServiceConfigurationScriptBuilder.BuildEncodedCommand(
            [CreateService("BITS", "AutomaticDelayedStart")],
            reportPath);

        string script = Encoding.Unicode.GetString(Convert.FromBase64String(command));
        Match payloadMatch = Regex.Match(
            script,
            "FromBase64String\\('(?<payload>[A-Za-z0-9+/=]+)'\\)",
            RegexOptions.None,
            TimeSpan.FromSeconds(1));

        Assert.True(payloadMatch.Success);
        Assert.DoesNotContain("BITS", script, StringComparison.Ordinal);
        Assert.Contains("Get-Service -Name $change.Name", script, StringComparison.Ordinal);

        using var compressed = new MemoryStream(Convert.FromBase64String(payloadMatch.Groups["payload"].Value));
        using var gzip = new GZipStream(compressed, CompressionMode.Decompress);
        using var payloadStream = new MemoryStream();
        gzip.CopyTo(payloadStream);
        using JsonDocument payload = JsonDocument.Parse(Encoding.UTF8.GetString(payloadStream.ToArray()));
        Assert.Equal(reportPath, payload.RootElement.GetProperty("ReportPath").GetString());
        Assert.Equal("BITS", payload.RootElement.GetProperty("Changes")[0].GetProperty("Name").GetString());
        Assert.Equal("AutomaticDelayedStart", payload.RootElement.GetProperty("Changes")[0].GetProperty("StartupType").GetString());
    }

    [Fact]
    public void BuildEncodedCommand_RejectsEmptySelection()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceConfigurationScriptBuilder.BuildEncodedCommand([], Path.GetFullPath("report.json")));
    }

    [Fact]
    public void BuildEncodedCommand_RejectsRelativeReportPath()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceConfigurationScriptBuilder.BuildEncodedCommand([CreateService("BITS", "Manual")], "report.json"));
    }

    [Fact]
    public void BuildEncodedCommand_RejectsPayloadsBeyondWindowsCommandLineLimit()
    {
        Servizio[] changes = Enumerable.Range(0, 512)
            .Select(index => CreateService($"service-{index:D4}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(120))}", "Manual"))
            .ToArray();

        Assert.Throws<InvalidDataException>(() =>
            ServiceConfigurationScriptBuilder.BuildEncodedCommand(changes, Path.GetFullPath("report.json")));
    }

    [Fact]
    public void BuildEncodedCommand_AcceptsTheCurrentFullServiceCatalogSize()
    {
        Servizio[] changes = Enumerable.Range(0, 164)
            .Select(index => CreateService($"service-{index:D3}-{new string('x', 24)}", "AutomaticDelayedStart"))
            .ToArray();

        string command = ServiceConfigurationScriptBuilder.BuildEncodedCommand(changes, Path.GetFullPath("report.json"));

        Assert.True(command.Length <= 28_000);
    }

    [Fact]
    public async Task BuildEncodedCommand_ProducesValidWindowsPowerShellSyntax()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        string command = ServiceConfigurationScriptBuilder.BuildEncodedCommand(
            [CreateService("BITS", "AutomaticDelayedStart")],
            Path.GetFullPath("report.json"));
        string parserScript = "$source = [Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('" + command + "')); " +
            "$tokens = $null; $errors = $null; " +
            "[System.Management.Automation.Language.Parser]::ParseInput($source, [ref]$tokens, [ref]$errors) | Out-Null; " +
            "if ($errors.Count -gt 0) { $errors | ForEach-Object { [Console]::Error.WriteLine($_.Message) }; exit 1 }";
        string encodedParserScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(parserScript));
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encodedParserScript);

        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        using Process process = Process.Start(startInfo)!;
        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> standardError = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        string output = await standardOutput;
        string error = await standardError;

        Assert.True(process.ExitCode == 0, $"PowerShell parser rejected generated script. stdout: {output}; stderr: {error}");
    }

    [Fact]
    public async Task BuildEncodedCommand_AppliesChangeAndWritesReportWithMockedServices()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        (int exitCode, List<ServiceConfigurationResult> results) = await RunMockedCommandAsync(
            [CreateService("BITS", "AutomaticDelayedStart")],
            failServiceName: null);

        Assert.True(exitCode == 0, $"PowerShell reported {results[0].Error}");
        Assert.Single(results);
        Assert.True(results[0].Success, results[0].Error);
        Assert.Equal("BITS", results[0].ServiceName);
    }

    [Fact]
    public async Task BuildEncodedCommand_ReportsPartialFailuresAndContinues()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        (int exitCode, List<ServiceConfigurationResult> results) = await RunMockedCommandAsync(
            [CreateService("first", "Manual"), CreateService("broken", "Disabled"), CreateService("last", "Automatic")],
            failServiceName: "broken");

        Assert.Equal(1, exitCode);
        Assert.Equal(3, results.Count);
        Assert.True(results[0].Success, results[0].Error);
        Assert.False(results[1].Success);
        Assert.True(results[2].Success);
    }

    private static async Task<(int ExitCode, List<ServiceConfigurationResult> Results)> RunMockedCommandAsync(
        IReadOnlyCollection<Servizio> changes,
        string? failServiceName)
    {
        string reportPath = Path.Combine(Path.GetTempPath(), $"WinHubX-ServiceScriptTests-{Guid.NewGuid():N}.json");
        string encodedCommand = ServiceConfigurationScriptBuilder.BuildEncodedCommand(changes, reportPath);
        string failureName = Convert.ToBase64String(Encoding.UTF8.GetBytes(failServiceName ?? string.Empty));
        string wrapperScript = """
            $source = [Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('__COMMAND__'))
            $failureName = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('__FAILURE__'))
            function global:Get-Service {
                [CmdletBinding()]
                param([string]$Name)
                [pscustomobject]@{ Name = $Name; Status = 'Stopped' }
            }
            function global:Stop-Service { throw 'Unexpected stop request for a stopped mock service.' }
            function global:Set-Service {
                [CmdletBinding()]
                param($InputObject, [string]$StartupType)
                if ($InputObject.Name -ceq $failureName) { throw 'Mocked service failure.' }
            }
            & ([System.Management.Automation.ScriptBlock]::Create($source))
            """.Replace("__COMMAND__", encodedCommand, StringComparison.Ordinal)
            .Replace("__FAILURE__", failureName, StringComparison.Ordinal);
        string encodedWrapper = Convert.ToBase64String(Encoding.Unicode.GetBytes(wrapperScript));
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encodedWrapper);

        try
        {
            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start Windows PowerShell for the script test.");
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Task<string> standardOutput = process.StandardOutput.ReadToEndAsync(cancellationToken);
            Task<string> standardError = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);
            string output = await standardOutput;
            string error = await standardError;
            Assert.True(process.ExitCode is 0 or 1, $"PowerShell failed. stdout: {output}; stderr: {error}");

            string reportJson = await File.ReadAllTextAsync(reportPath, cancellationToken);
            List<ServiceConfigurationResult> results = System.Text.Json.JsonSerializer.Deserialize<List<ServiceConfigurationResult>>(
                reportJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            return (process.ExitCode, results);
        }
        finally
        {
            File.Delete(reportPath);
        }
    }

    private static Servizio CreateService(string name, string startupType) => new()
    {
        Name = name,
        StartupType = startupType,
        OriginalType = "Manual"
    };
}
