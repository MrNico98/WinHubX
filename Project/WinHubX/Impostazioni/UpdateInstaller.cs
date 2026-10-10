using System.Security.Cryptography;

namespace WinHubX.Impostazioni;

public static class UpdateInstaller
{
    public static async Task InstallAndStartAsync(
        string downloadedFilePath,
        string executablePath,
        string expectedSha256,
        Func<string, CancellationToken, Task<bool>> startAndWaitForReadyAsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(downloadedFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        ArgumentNullException.ThrowIfNull(startAndWaitForReadyAsync);
        if (!UpdateManifestValidator.IsValidSha256(expectedSha256))
        {
            throw new InvalidDataException("SHA-256 dell'aggiornamento assente o non valido.");
        }

        string sourcePath = Path.GetFullPath(downloadedFilePath);
        string destinationPath = Path.GetFullPath(executablePath);
        if (string.Equals(sourcePath, destinationPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Il file sorgente non può coincidere con l'eseguibile installato.");
        }
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("Pacchetto aggiornamento non trovato.", sourcePath);
        }

        string? destinationDirectory = Path.GetDirectoryName(destinationPath);
        if (string.IsNullOrWhiteSpace(destinationDirectory) || !Directory.Exists(destinationDirectory))
        {
            throw new DirectoryNotFoundException("La directory dell'applicazione non è disponibile.");
        }

        string stagingPath = Path.Join(destinationDirectory, $".{Path.GetFileName(destinationPath)}.{Guid.NewGuid():N}.pending");
        string backupPath = $"{destinationPath}.{Guid.NewGuid():N}.rollback";
        bool replaced = false;

        try
        {
            File.Copy(sourcePath, stagingPath, overwrite: false);
            await VerifySha256Async(stagingPath, expectedSha256, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (File.Exists(destinationPath))
            {
                File.Replace(stagingPath, destinationPath, backupPath);
            }
            else
            {
                File.Move(stagingPath, destinationPath);
            }
            replaced = true;

            bool startedAndReady = await startAndWaitForReadyAsync(destinationPath, cancellationToken);
            if (!startedAndReady)
            {
                throw new InvalidOperationException("La nuova versione non ha avviato correttamente la finestra principale.");
            }
        }
        catch (Exception updateException)
        {
            if (replaced)
            {
                try
                {
                    if (File.Exists(backupPath))
                    {
                        File.Move(backupPath, destinationPath, overwrite: true);
                    }
                    else if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                }
                catch (Exception rollbackException) when (rollbackException is IOException or UnauthorizedAccessException)
                {
                    throw new AggregateException(
                        "L'aggiornamento è fallito e il rollback automatico non è riuscito.",
                        updateException,
                        rollbackException);
                }
            }

            throw;
        }
        finally
        {
            TryDelete(stagingPath);
        }

        TryDelete(backupPath);
    }

    private static async Task VerifySha256Async(string path, string expectedSha256, CancellationToken cancellationToken)
    {
        await using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
        byte[] actualHash = await SHA256.HashDataAsync(stream, cancellationToken);
        byte[] expectedHash = Convert.FromHexString(expectedSha256);
        if (!CryptographicOperations.FixedTimeEquals(actualHash, expectedHash))
        {
            throw new InvalidDataException("Il controllo SHA-256 del file staged non è riuscito.");
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            System.Diagnostics.Debug.WriteLine($"File temporaneo updater non eliminato: {exception}");
        }
    }
}
