using Microsoft.Win32;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace WinHubX.Impostazioni;

[SupportedOSPlatform("windows")]
internal sealed class ElevatedRegistryAclMutationBatch
{
    private static readonly HashSet<string> AllowedServiceKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        @"SYSTEM\CurrentControlSet\Services\WinDefend",
        @"SYSTEM\CurrentControlSet\Services\WdNisSvc",
        @"SYSTEM\CurrentControlSet\Services\Sense"
    };

    private readonly List<AclMutation> _mutations = [];

    internal int Count => _mutations.Count;

    internal void AddLocalMachineTakeOwnership(string subKeyPath, RegistryView view, string userSid)
    {
        if (!AllowedServiceKeys.Contains(subKeyPath))
            throw new ArgumentException("La modifica ACL è consentita solo alle chiavi dei servizi Defender previsti.", nameof(subKeyPath));
        if (view is not (RegistryView.Registry32 or RegistryView.Registry64))
            throw new ArgumentOutOfRangeException(nameof(view));
        if (_mutations.Count >= 16)
            throw new InvalidOperationException("Il batch ACL supera il numero massimo di modifiche.");
        try
        {
            _ = new SecurityIdentifier(userSid);
        }
        catch (ArgumentException exception)
        {
            throw new ArgumentException("Il SID utente non è valido.", nameof(userSid), exception);
        }

        _mutations.Add(new AclMutation(subKeyPath, view.ToString(), userSid));
    }

    internal string BuildCommand()
    {
        if (_mutations.Count == 0)
            throw new InvalidOperationException("Il batch ACL non contiene modifiche.");

        string json = JsonSerializer.Serialize(_mutations);
        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return $$"""
            $ErrorActionPreference = 'Stop'
            $aclOperations = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('{{payload}}')) | ConvertFrom-Json
            foreach ($operation in $aclOperations) {
                $view = [Microsoft.Win32.RegistryView][Enum]::Parse([Microsoft.Win32.RegistryView], [string]$operation.View)
                $baseKey = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, $view)
                try {
                    $rights = [System.Security.AccessControl.RegistryRights]::ReadPermissions -bor [System.Security.AccessControl.RegistryRights]::ChangePermissions
                    $key = $baseKey.OpenSubKey([string]$operation.Path, [Microsoft.Win32.RegistryKeyPermissionCheck]::ReadWriteSubTree, $rights)
                    if ($null -eq $key) { throw "La chiave ACL richiesta non esiste." }
                    try {
                        $acl = $key.GetAccessControl()
                        $sid = New-Object System.Security.Principal.SecurityIdentifier -ArgumentList ([string]$operation.UserSid)
                        $rule = New-Object System.Security.AccessControl.RegistryAccessRule -ArgumentList $sid, [System.Security.AccessControl.RegistryRights]::TakeOwnership, [System.Security.AccessControl.AccessControlType]::Allow
                        $acl.AddAccessRule($rule) | Out-Null
                        $key.SetAccessControl($acl)
                    }
                    finally { $key.Dispose() }
                }
                finally { $baseKey.Dispose() }
            }
            """;
    }

    private sealed record AclMutation(string Path, string View, string UserSid);
}
