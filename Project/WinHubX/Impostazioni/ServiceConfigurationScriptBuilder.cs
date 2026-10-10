using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace WinHubX.Impostazioni;

public static class ServiceConfigurationScriptBuilder
{
    private const int MaximumEncodedCommandLength = 28_000;

    public static string BuildEncodedCommand(IReadOnlyCollection<Servizio> changes, string reportPath)
    {
        ArgumentNullException.ThrowIfNull(changes);

        if (changes.Count == 0)
        {
            throw new ArgumentException("È necessario selezionare almeno un servizio.", nameof(changes));
        }

        if (!Path.IsPathFullyQualified(reportPath))
        {
            throw new ArgumentException("Il percorso del report deve essere assoluto.", nameof(reportPath));
        }

        ServiceConfigurationValidator.ValidateCatalog(new ServiziRoot { service = changes.ToList() });

        string payload = JsonSerializer.Serialize(new
        {
            Changes = changes.Select(static change => new { change.Name, change.StartupType }).ToArray(),
            ReportPath = reportPath
        });
        using var compressedPayload = new MemoryStream();
        using (var gzip = new GZipStream(compressedPayload, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(Encoding.UTF8.GetBytes(payload));
        }

        string payloadBase64 = Convert.ToBase64String(compressedPayload.ToArray());

        string script = """
            $ErrorActionPreference = 'Stop'
            $compressedPayload = [IO.MemoryStream]::new([Convert]::FromBase64String('__PAYLOAD__'))
            $gzip = [IO.Compression.GZipStream]::new($compressedPayload, [IO.Compression.CompressionMode]::Decompress)
            $payloadStream = [IO.MemoryStream]::new()
            $gzip.CopyTo($payloadStream)
            $gzip.Dispose()
            $compressedPayload.Dispose()
            $payload = [Text.Encoding]::UTF8.GetString($payloadStream.ToArray())
            $payloadStream.Dispose()
            $data = ConvertFrom-Json -InputObject $payload
            $results = [System.Collections.Generic.List[object]]::new()
            $hasFailures = $false
            foreach ($change in $data.Changes) {
                try {
                    $service = Get-Service -Name $change.Name -ErrorAction Stop | Where-Object { $_.Name -ceq $change.Name } | Select-Object -First 1
                    if ($null -eq $service) { throw 'Servizio non trovato.' }
                    if ($service.Status.ToString() -ne 'Stopped') {
                        Stop-Service -InputObject $service -ErrorAction Stop -Confirm:$false
                    }
                    Set-Service -InputObject $service -StartupType $change.StartupType -ErrorAction Stop
                    $results.Add([pscustomobject]@{ ServiceName = $change.Name; Success = $true; Error = $null })
                }
                catch {
                    $hasFailures = $true
                    $results.Add([pscustomobject]@{ ServiceName = $change.Name; Success = $false; Error = $_.Exception.Message })
                }
            }
            $json = ConvertTo-Json -InputObject @($results.ToArray()) -Compress -Depth 3
            [IO.File]::WriteAllText($data.ReportPath, $json, [Text.UTF8Encoding]::new($false))
            if ($hasFailures) { exit 1 }
            exit 0
            """.Replace("__PAYLOAD__", payloadBase64, StringComparison.Ordinal);

        string encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        if (encodedCommand.Length > MaximumEncodedCommandLength)
        {
            throw new InvalidDataException("La selezione supera la dimensione massima consentita per il comando elevato.");
        }

        return encodedCommand;
    }
}

public sealed class ServiceConfigurationResult
{
    public string ServiceName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
