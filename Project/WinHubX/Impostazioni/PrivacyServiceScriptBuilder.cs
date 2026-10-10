using System.Text;
using System.Text.Json;
using System.Runtime.Versioning;

namespace WinHubX.Impostazioni;

internal sealed record PrivacyServiceChange(string Name, string StartupType, bool StartAfterConfiguration);

[SupportedOSPlatform("windows")]
internal static class PrivacyServiceScriptBuilder
{
    private const int MaximumServiceChanges = 4;
    private static readonly HashSet<string> AllowedServiceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "DiagTrack",
        "dmwappushservice",
        "HomeGroupListener",
        "HomeGroupProvider"
    };
    private static readonly HashSet<string> AllowedStartupTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Automatic",
        "Manual",
        "Disabled"
    };

    internal static string BuildEncodedCommand(IReadOnlyCollection<PrivacyServiceChange> changes)
        => BuildEncodedCommand(changes, registryChanges: null);

    internal static string BuildEncodedCommand(
        IReadOnlyCollection<PrivacyServiceChange> changes,
        ElevatedRegistryMutationBatch? registryChanges)
    {
        ArgumentNullException.ThrowIfNull(changes);
        if (changes.Count is 0 or > MaximumServiceChanges)
            throw new ArgumentOutOfRangeException(nameof(changes), "Numero di modifiche servizio non valido.");
        foreach (PrivacyServiceChange change in changes)
        {
            if (!AllowedServiceNames.Contains(change.Name))
                throw new ArgumentException($"Servizio non consentito: {change.Name}.", nameof(changes));
            if (!AllowedStartupTypes.Contains(change.StartupType))
                throw new ArgumentException($"Tipo di avvio non consentito: {change.StartupType}.", nameof(changes));
        }

        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(changes)));
        string script = $$"""
            $ErrorActionPreference = 'Stop'
            $changes = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('{{payload}}')) | ConvertFrom-Json
            foreach ($change in $changes) {
                $service = Get-Service -Name ([string]$change.Name) -ErrorAction Stop
                if (-not [bool]$change.StartAfterConfiguration -and $service.Status -ne 'Stopped') {
                    Stop-Service -InputObject $service -ErrorAction Stop -Confirm:$false
                }
                Set-Service -InputObject $service -StartupType ([string]$change.StartupType) -ErrorAction Stop
                if ([bool]$change.StartAfterConfiguration) {
                    $service = Get-Service -Name ([string]$change.Name) -ErrorAction Stop
                    if ($service.Status -ne 'Running') {
                        Start-Service -InputObject $service -ErrorAction Stop
                    }
                }
            }
            """;
        if (registryChanges is { Count: > 0 })
            script = registryChanges.BuildCommand() + Environment.NewLine + script;
        return Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
    }
}
