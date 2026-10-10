using System.IO.Compression;

namespace WinHubX.Impostazioni;

internal static class SafeZipExtractor
{
    private const int MaximumEntryCount = 20_000;
    private const long MaximumEntryLength = 4L * 1024 * 1024 * 1024;
    private const long MaximumExpandedLength = 8L * 1024 * 1024 * 1024;

    internal static void ExtractToFreshDirectory(string archivePath, string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        string root = Path.GetFullPath(destinationDirectory);
        if (Directory.Exists(root) || File.Exists(root))
            throw new IOException("La directory di estrazione deve essere nuova.");

        using ZipArchive archive = ZipFile.OpenRead(archivePath);
        if (archive.Entries.Count > MaximumEntryCount)
            throw new InvalidDataException("L'archivio contiene troppe voci.");

        long expandedLength = 0;
        Directory.CreateDirectory(root);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            ValidateEntry(entry);
            expandedLength = checked(expandedLength + entry.Length);
            if (entry.Length > MaximumEntryLength || expandedLength > MaximumExpandedLength)
                throw new InvalidDataException("L'archivio supera i limiti di estrazione consentiti.");

            string destinationPath;
            try
            {
                destinationPath = SafePathResolver.ResolveContainedPath(root, entry.FullName);
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidDataException($"Percorso ZIP non consentito: {entry.FullName}", ex);
            }

            if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
            {
                Directory.CreateDirectory(destinationPath);
                continue;
            }

            string? parentDirectory = Path.GetDirectoryName(destinationPath);
            if (parentDirectory is not null)
                Directory.CreateDirectory(parentDirectory);
            entry.ExtractToFile(destinationPath, overwrite: false);
        }
    }

    private static void ValidateEntry(ZipArchiveEntry entry)
    {
        string name = entry.FullName;
        if (string.IsNullOrWhiteSpace(name)
            || name.Any(char.IsControl)
            || name.Contains(':', StringComparison.Ordinal)
            || Path.IsPathRooted(name)
            || name.StartsWith('/')
            || name.StartsWith('\\'))
            throw new InvalidDataException("L'archivio contiene un percorso assoluto o non valido.");

        foreach (string segment in name.Replace('\\', '/').Split('/'))
        {
            if (segment is "." or "..")
                throw new InvalidDataException("L'archivio contiene un segmento di percorso non consentito.");
        }

        int unixFileType = (entry.ExternalAttributes >> 16) & 0xF000;
        if (unixFileType == 0xA000 || (entry.ExternalAttributes & (int)FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("L'archivio contiene link simbolici o reparse point.");
    }
}
