using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WinHubX.Impostazioni;

[SupportedOSPlatform("windows")]
internal static class PrivateUserWorkspace
{
    private const string WorkspaceFolderName = "WinHubX\\PrivateSessions";

    internal static string CreateSession()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localAppData))
            throw new IOException("Impossibile determinare la cartella dati locale dell'utente.");

        string appRoot = Path.Join(localAppData, "WinHubX");
        Directory.CreateDirectory(appRoot);
        EnsureNotReparsePoint(appRoot);
        string parentPath = Path.Join(appRoot, "PrivateSessions");
        Directory.CreateDirectory(parentPath);
        EnsureNotReparsePoint(parentPath);

        string sessionPath = Path.Join(parentPath, Guid.NewGuid().ToString("N"));
        SecurityIdentifier userSid = WindowsIdentity.GetCurrent().User
            ?? throw new UnauthorizedAccessException("Impossibile determinare l'identità Windows corrente.");
        DirectorySecurity security = new();
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.SetOwner(userSid);
        AddFullControl(security, userSid);
        AddFullControl(security, new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
        AddFullControl(security, new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null));

        DirectoryInfo session = Directory.CreateDirectory(sessionPath);
        session.SetAccessControl(security);
        EnsureNotReparsePoint(sessionPath);
        return sessionPath;
    }

    internal static void DeleteSession(string sessionPath)
    {
        string fullPath = ValidateSessionPath(sessionPath);
        if (!Directory.Exists(fullPath))
            return;

        EnsureNotReparsePoint(fullPath);
        Directory.Delete(fullPath, recursive: true);
    }

    internal static string ValidateSessionPath(string sessionPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionPath);
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localAppData))
            throw new IOException("Impossibile determinare la cartella dati locale dell'utente.");

        string parentPath = Path.GetFullPath(Path.Join(localAppData, WorkspaceFolderName));
        string fullPath = Path.GetFullPath(sessionPath);
        if (!string.Equals(Path.GetDirectoryName(fullPath), parentPath, StringComparison.OrdinalIgnoreCase)
            || !Guid.TryParseExact(Path.GetFileName(fullPath), "N", out _))
        {
            throw new InvalidDataException("Il percorso della sessione privata non è valido.");
        }

        return fullPath;
    }

    private static void AddFullControl(DirectorySecurity security, SecurityIdentifier identity) =>
        security.AddAccessRule(new FileSystemAccessRule(
            identity,
            FileSystemRights.FullControl,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None,
            AccessControlType.Allow));

    private static void EnsureNotReparsePoint(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new IOException("Il workspace privato non può contenere reparse point.");
    }
}
