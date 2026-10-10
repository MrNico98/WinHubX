using Microsoft.Win32;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace WinHubX.Impostazioni;

[SupportedOSPlatform("windows")]
internal sealed class ElevatedRegistryMutationBatch
{
    private const int MaximumMutationCount = 256;
    private const int MaximumSerializedPayloadBytes = 16 * 1024;
    private readonly List<RegistryMutation> _mutations = [];

    internal int Count => _mutations.Count;

    internal void SetValue(
        RegistryHive hive,
        string subKeyPath,
        string valueName,
        object value,
        RegistryValueKind valueKind,
        RegistryView view)
    {
        ArgumentNullException.ThrowIfNull(value);
        ValidateTarget(hive, subKeyPath, valueName, view);
        if (valueKind is not (RegistryValueKind.String or RegistryValueKind.ExpandString
            or RegistryValueKind.DWord or RegistryValueKind.QWord or RegistryValueKind.MultiString or RegistryValueKind.Binary))
        {
            throw new ArgumentOutOfRangeException(nameof(valueKind), "Tipo di valore Registro non supportato.");
        }
        bool valueMatchesKind = valueKind switch
        {
            RegistryValueKind.String or RegistryValueKind.ExpandString => value is string,
            RegistryValueKind.DWord => value is int or uint,
            RegistryValueKind.QWord => value is long or ulong,
            RegistryValueKind.MultiString => value is string[],
            RegistryValueKind.Binary => value is byte[],
            _ => false
        };
        if (!valueMatchesKind)
            throw new ArgumentException("Il valore non corrisponde al tipo Registro specificato.", nameof(value));

        _mutations.Add(new RegistryMutation(hive.ToString(), view.ToString(), subKeyPath, valueName,
            valueKind.ToString(), value, Delete: false));
    }

    internal void DeleteValue(RegistryHive hive, string subKeyPath, string valueName, RegistryView view)
    {
        ValidateTarget(hive, subKeyPath, valueName, view);
        _mutations.Add(new RegistryMutation(hive.ToString(), view.ToString(), subKeyPath, valueName,
            string.Empty, null, Delete: true));
    }

    internal string BuildCommand()
    {
        if (_mutations.Count == 0)
            throw new InvalidOperationException("Il batch Registro non contiene modifiche.");
        if (_mutations.Count > MaximumMutationCount)
            throw new InvalidOperationException("Il batch Registro supera il numero massimo di modifiche.");

        string json = JsonSerializer.Serialize(_mutations);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        if (jsonBytes.Length > MaximumSerializedPayloadBytes)
            throw new InvalidOperationException("Il batch Registro supera la dimensione massima consentita.");
        string payload = Convert.ToBase64String(jsonBytes);
        string script = $$"""
            $ErrorActionPreference = 'Stop'
            $payload = '{{payload}}'
            $mutations = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload)) | ConvertFrom-Json
            foreach ($mutation in $mutations) {
                $hive = [Microsoft.Win32.RegistryHive][Enum]::Parse([Microsoft.Win32.RegistryHive], [string]$mutation.Hive)
                $view = [Microsoft.Win32.RegistryView][Enum]::Parse([Microsoft.Win32.RegistryView], [string]$mutation.View)
                $baseKey = [Microsoft.Win32.RegistryKey]::OpenBaseKey($hive, $view)
                try {
                    if ($mutation.Delete) {
                        $key = $baseKey.OpenSubKey([string]$mutation.Path, $true)
                        if ($null -ne $key) {
                            try { $key.DeleteValue([string]$mutation.Name, $false) }
                            finally { $key.Dispose() }
                        }
                    }
                    else {
                        $key = $baseKey.CreateSubKey([string]$mutation.Path, $true)
                        if ($null -eq $key) { throw "Impossibile aprire la chiave Registro richiesta." }
                        try {
                            $kind = [Microsoft.Win32.RegistryValueKind][Enum]::Parse([Microsoft.Win32.RegistryValueKind], [string]$mutation.Kind)
                            $value = switch ($kind) {
                                DWord {
                                    $unsignedValue = [uint32]$mutation.Value
                                    [System.BitConverter]::ToInt32([System.BitConverter]::GetBytes($unsignedValue), 0)
                                    break
                                }
                                QWord { [long]$mutation.Value; break }
                                MultiString { [string[]]$mutation.Value; break }
                                Binary { [Convert]::FromBase64String([string]$mutation.Value); break }
                                default { [string]$mutation.Value }
                            }
                            $key.SetValue([string]$mutation.Name, $value, $kind)
                        }
                        finally { $key.Dispose() }
                    }
                }
                finally { $baseKey.Dispose() }
            }
            """;

        return script;
    }

    private static void ValidateTarget(RegistryHive hive, string subKeyPath, string valueName, RegistryView view)
    {
        if (hive is not (RegistryHive.LocalMachine or RegistryHive.Users))
            throw new ArgumentOutOfRangeException(nameof(hive), "Il batch elevato accetta solo HKLM e HKEY_USERS.");
        if (string.IsNullOrWhiteSpace(subKeyPath)
            || subKeyPath.StartsWith('\\')
            || subKeyPath.Split('\\').Any(static segment => segment is "" or "." or "..")
            || subKeyPath.Any(char.IsControl))
        {
            throw new ArgumentException("Percorso Registro non valido.", nameof(subKeyPath));
        }
        if (string.IsNullOrWhiteSpace(valueName) || valueName.Any(char.IsControl))
            throw new ArgumentException("Nome valore Registro non valido.", nameof(valueName));
        if (view is not (RegistryView.Default or RegistryView.Registry32 or RegistryView.Registry64))
            throw new ArgumentOutOfRangeException(nameof(view));
    }

    private sealed record RegistryMutation(
        string Hive,
        string View,
        string Path,
        string Name,
        string Kind,
        object? Value,
        bool Delete);
}
