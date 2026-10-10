namespace WinHubX.Impostazioni;

internal static class SafePathResolver
{
    internal static string ResolveContainedPath(string rootDirectory, string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        if (Path.IsPathRooted(relativePath)
            || relativePath.Contains(':', StringComparison.Ordinal)
            || relativePath.Any(char.IsControl))
        {
            throw new InvalidDataException("Il percorso relativo contiene elementi non validi.");
        }

        string normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        if (normalizedPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(static segment => segment is "." or ".."))
        {
            throw new InvalidDataException("Il percorso relativo tenta di uscire dalla directory consentita.");
        }

        string fullRoot = Path.GetFullPath(rootDirectory);
        string rootPrefix = Path.EndsInDirectorySeparator(fullRoot)
            ? fullRoot
            : fullRoot + Path.DirectorySeparatorChar;
        string fullPath = Path.GetFullPath(Path.Join(fullRoot, normalizedPath));
        StringComparison comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!fullPath.StartsWith(rootPrefix, comparison))
            throw new InvalidDataException("Il percorso richiesto non è contenuto nella directory consentita.");

        return fullPath;
    }
}
