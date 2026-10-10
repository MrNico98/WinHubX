using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace WinHubX.Impostazioni;

internal sealed record PrivacyScheduledTaskChange(string Name, bool Enable);

[SupportedOSPlatform("windows")]
internal static class PrivacyScheduledTaskScriptBuilder
{
    private const int MaximumTaskChanges = 16;
    private static readonly HashSet<string> AllowedTaskNames = new(StringComparer.OrdinalIgnoreCase)
    {
        @"Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser",
        @"Microsoft\Windows\Application Experience\ProgramDataUpdater",
        @"Microsoft\Windows\Autochk\Proxy",
        @"Microsoft\Windows\Customer Experience Improvement Program\Consolidator",
        @"Microsoft\Windows\Customer Experience Improvement Program\UsbCeip",
        @"Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector",
        @"Microsoft\Windows\Feedback\Siuf\DmClient",
        @"Microsoft\Windows\Feedback\Siuf\DmClientOnScenarioDownload",
        @"Microsoft\Windows\Windows Error Reporting\QueueReporting",
        @"Microsoft\Windows\Application Experience\MareBackup",
        @"Microsoft\Windows\Application Experience\StartupAppTask",
        @"Microsoft\Windows\Application Experience\PcaPatchDbTask",
        @"Microsoft\Windows\Maps\MapsUpdateTask",
        @"Microsoft\Windows\Defrag\ScheduledDefrag"
    };

    internal static string BuildScript(IReadOnlyCollection<PrivacyScheduledTaskChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);
        if (changes.Count is 0 or > MaximumTaskChanges)
            throw new ArgumentOutOfRangeException(nameof(changes), "Numero di attività pianificate non valido.");

        foreach (PrivacyScheduledTaskChange change in changes)
        {
            ArgumentNullException.ThrowIfNull(change);
            if (!AllowedTaskNames.Contains(change.Name))
                throw new ArgumentException($"Attività pianificata non consentita: {change.Name}.", nameof(changes));
        }

        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(changes)));
        return $$"""
            $ErrorActionPreference = 'Stop'
            $payload = '{{payload}}'
            $changes = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload)) | ConvertFrom-Json
            foreach ($change in $changes) {
                $taskIdentifier = [string]$change.Name
                $separatorIndex = $taskIdentifier.LastIndexOf('\')
                $taskPath = '\' + $taskIdentifier.Substring(0, $separatorIndex + 1)
                $taskName = $taskIdentifier.Substring($separatorIndex + 1)
                $task = Get-ScheduledTask -TaskName $taskName -TaskPath $taskPath -ErrorAction SilentlyContinue
                if ($null -eq $task) { continue }
                if ([bool]$change.Enable -and $task.State -eq 'Disabled') {
                    Enable-ScheduledTask -TaskName $taskName -TaskPath $taskPath -ErrorAction Stop | Out-Null
                }
                elseif (-not [bool]$change.Enable -and $task.State -ne 'Disabled') {
                    Disable-ScheduledTask -TaskName $taskName -TaskPath $taskPath -ErrorAction Stop | Out-Null
                }
            }
            """;
    }
}
