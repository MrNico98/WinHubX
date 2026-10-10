namespace WinHubX.Impostazioni;

internal static class RegistryPresetFileValidator
{
    private const long MaximumPresetSizeBytes = 1024 * 1024;
    private const string AllowedRegistryKey = @"HKEY_CURRENT_USER\Software\WinHubX";

    internal static void Validate(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        Validate(stream);
    }

    internal static void Validate(FileStream stream)
    {
        if (!stream.CanRead || !stream.CanSeek)
            throw new ArgumentException("Il file del preset deve essere leggibile e ricercabile.", nameof(stream));

        if (stream.Length == 0 || stream.Length > MaximumPresetSizeBytes)
            throw new InvalidDataException("Il file di configurazione è vuoto o supera il limite di 1 MiB.");

        stream.Position = 0;
        using StreamReader reader = new(stream, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        string? header = reader.ReadLine();
        if (header?.Trim() is not ("Windows Registry Editor Version 5.00" or "REGEDIT4"))
            throw new InvalidDataException("Il file non è un'esportazione valida del Registro di Windows.");

        bool hasSettingsKey = false;
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            string section = line.Trim();
            if (section.Length == 0 || section[0] != '[')
                continue;

            if (section.Length < 2 || section[^1] != ']')
                throw new InvalidDataException("Il file contiene una sezione del Registro malformata.");

            string registryKey = section[1..^1];
            if (registryKey.Length > 0 && registryKey[0] == '-')
                throw new InvalidDataException("I preset WinHubX non possono eliminare chiavi del Registro.");

            if (!registryKey.Equals(AllowedRegistryKey, StringComparison.OrdinalIgnoreCase)
                && !registryKey.StartsWith(AllowedRegistryKey + "\\", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Il file può contenere solo le impostazioni sotto HKCU\\Software\\WinHubX.");
            }

            hasSettingsKey = true;
        }

        if (!hasSettingsKey)
            throw new InvalidDataException("Il file non contiene impostazioni WinHubX da importare.");
    }
}
