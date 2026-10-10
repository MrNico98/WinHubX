using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace WinHubX.Impostazioni;

internal static class BitsTransferDownloader
{
    private static readonly HttpClient RedirectClient = new(new SocketsHttpHandler
    {
        AllowAutoRedirect = false,
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
    }) { Timeout = TimeSpan.FromMinutes(2) };

    internal static async Task DownloadFileAsync(
        string url,
        string destinationPath,
        Action<int>? onProgress,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        Uri resolvedUri = await TrustedHttpsClient.ResolveDownloadUriAsync(RedirectClient, url, cancellationToken);
        string fullDestination = Path.GetFullPath(destinationPath);
        string? directory = Path.GetDirectoryName(fullDestination);
        if (directory is null)
            throw new IOException("Impossibile determinare la cartella di destinazione del download.");
        Directory.CreateDirectory(directory);

        string temporaryPath = $"{fullDestination}.{Guid.NewGuid():N}.bits";
        string jobName = $"WinHubX-{Guid.NewGuid():N}";
        try
        {
            await RunPowerShellAsync(BuildTransferScript(resolvedUri, temporaryPath, jobName), onProgress, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (!File.Exists(temporaryPath))
                throw new IOException("BITS ha completato il processo senza creare il file richiesto.");

            File.Move(temporaryPath, fullDestination, overwrite: true);
            onProgress?.Invoke(100);
        }
        catch
        {
            await CancelJobAsync(jobName);
            TryDelete(temporaryPath);
            throw;
        }
    }

    internal static string BuildTransferScript(Uri source, string destination, string jobName)
    {
        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            Source = source.AbsoluteUri,
            Destination = destination,
            JobName = jobName
        })));

        string script = """
            $ErrorActionPreference = 'Stop'
            $payload = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('__PAYLOAD__')) | ConvertFrom-Json
            $job = $null
            $exitCode = 1
            try {
                $job = Start-BitsTransfer -Source $payload.Source -Destination $payload.Destination -Asynchronous `
                    -DisplayName $payload.JobName -Priority Foreground `
                    -SecurityFlags @('EnableCRLCheck', 'RedirectPolicyDisallow') `
                    -RetryInterval 10 -RetryTimeout 86400 -MaxDownloadTime 21600 -ErrorAction Stop
                $deadline = [DateTime]::UtcNow.AddHours(6)
                while ([DateTime]::UtcNow -lt $deadline) {
                    $job = Get-BitsTransfer -Name $payload.JobName -ErrorAction Stop
                    if ($job.BytesTotal -gt 0) {
                        $percent = [Math]::Min(99, [Math]::Floor(($job.BytesTransferred * 100.0) / $job.BytesTotal))
                        [Console]::Out.WriteLine(('BITS_PROGRESS:{0}' -f $percent))
                    }
                    switch ($job.JobState.ToString()) {
                        'Transferred' {
                            Complete-BitsTransfer -BitsJob $job -ErrorAction Stop
                            $job = $null
                            $exitCode = 0
                            break
                        }
                        'Error' { throw ('BITS: {0}' -f $job.ErrorDescription) }
                        'Cancelled' { throw 'Il job BITS è stato annullato.' }
                    }
                    if ($exitCode -eq 0) { break }
                    Start-Sleep -Milliseconds 500
                }
                if ($exitCode -ne 0) { throw 'Timeout durante il trasferimento BITS.' }
            }
            catch {
                [Console]::Error.WriteLine($_.Exception.Message)
                $exitCode = 1
            }
            finally {
                if ($null -ne $job) {
                    $remaining = Get-BitsTransfer -Name $payload.JobName -ErrorAction SilentlyContinue
                    if ($null -ne $remaining) { Remove-BitsTransfer -BitsJob $remaining -Confirm:$false -ErrorAction SilentlyContinue }
                }
            }
            exit $exitCode
            """.Replace("__PAYLOAD__", payload, StringComparison.Ordinal);

        return Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
    }

    private static async Task RunPowerShellAsync(string encodedScript, Action<int>? onProgress, CancellationToken cancellationToken)
    {
        string executablePath = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe");
        ProcessStartInfo startInfo = new(executablePath)
        {
            WorkingDirectory = Environment.SystemDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(encodedScript);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Impossibile avviare il client BITS.");
        Task progressTask = ReadProgressAsync(process.StandardOutput, onProgress);
        Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch (Exception ex) when (ex is InvalidOperationException or Win32Exception)
            {
                Debug.WriteLine($"Arresto del processo BITS annullato non riuscito: {ex.Message}");
            }

            await process.WaitForExitAsync(CancellationToken.None);
            await Task.WhenAll(progressTask, errorTask);
            throw;
        }

        await progressTask;
        string error = await errorTask;
        if (process.ExitCode != 0)
            throw new IOException($"Trasferimento BITS non riuscito (codice {process.ExitCode}): {error.Trim()}");
    }

    private static async Task ReadProgressAsync(StreamReader reader, Action<int>? onProgress)
    {
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            const string prefix = "BITS_PROGRESS:";
            if (line.StartsWith(prefix, StringComparison.Ordinal)
                && int.TryParse(line.AsSpan(prefix.Length), out int progress))
                onProgress?.Invoke(Math.Clamp(progress, 0, 100));
            else if (!string.IsNullOrWhiteSpace(line))
                Debug.WriteLine(line);
        }
    }

    private static async Task CancelJobAsync(string jobName)
    {
        try
        {
            string script = "Get-BitsTransfer -Name '" + jobName
                + "' -ErrorAction SilentlyContinue | Remove-BitsTransfer -Confirm:$false -ErrorAction SilentlyContinue";
            string encodedScript = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
            string executablePath = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe");
            ProcessStartInfo startInfo = new(executablePath)
            {
                WorkingDirectory = Environment.SystemDirectory,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedScript);
            using Process? process = Process.Start(startInfo);
            if (process is not null)
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or OperationCanceledException or TimeoutException)
        {
            Debug.WriteLine($"Pulizia job BITS non riuscita: {ex.Message}");
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"File temporaneo BITS non eliminato: {ex.Message}");
        }
    }
}
