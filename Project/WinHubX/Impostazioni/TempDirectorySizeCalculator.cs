using System.Diagnostics;

namespace WinHubX.Impostazioni;

public static class TempDirectorySizeCalculator
{
    public static long Calculate(string folderPath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        long size = 0;
        var pendingDirectories = new Stack<DirectoryInfo>();
        pendingDirectories.Push(new DirectoryInfo(folderPath));

        while (pendingDirectories.TryPop(out DirectoryInfo? currentDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();
            size += CalculateDirectorySize(currentDirectory, pendingDirectories, cancellationToken);
        }

        return size;
    }

    private static long CalculateDirectorySize(
        DirectoryInfo directory,
        Stack<DirectoryInfo> pendingDirectories,
        CancellationToken cancellationToken)
    {
        long size = 0;
        try
        {
            foreach (FileSystemInfo entry in directory.EnumerateFileSystemInfos())
            {
                cancellationToken.ThrowIfCancellationRequested();
                size += GetEntrySize(entry, pendingDirectories);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"Scansione cartella TEMP non riuscita: {ex.Message}");
        }

        return size;
    }

    private static long GetEntrySize(FileSystemInfo entry, Stack<DirectoryInfo> pendingDirectories)
    {
        try
        {
            FileAttributes attributes = entry.Attributes;
            bool isDirectory = (attributes & FileAttributes.Directory) != 0;
            bool isReparsePoint = (attributes & FileAttributes.ReparsePoint) != 0;

            if (isDirectory)
            {
                if (!isReparsePoint)
                    pendingDirectories.Push((DirectoryInfo)entry);

                return 0;
            }

            return !isReparsePoint && entry is FileInfo file ? file.Length : 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"Elemento TEMP non leggibile: {ex.Message}");
            return 0;
        }
    }
}
