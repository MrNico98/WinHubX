namespace WinHubX.Impostazioni;

public static class ServiceConfigurationValidator
{
    private const int MaximumServiceCount = 512;
    private const int MaximumServiceNameLength = 256;

    private static readonly HashSet<string> AllowedStartupTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Automatic",
        "AutomaticDelayedStart",
        "Manual",
        "Disabled"
    };

    public static void ValidateCatalog(ServiziRoot? catalog)
    {
        List<Servizio>? services = catalog?.service;
        if (services is null || services.Count is <= 0 or > MaximumServiceCount)
        {
            throw new InvalidDataException("Il catalogo servizi è vuoto o supera il limite consentito.");
        }

        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Servizio? service in services)
        {
            ValidateService(service);
            string serviceName = service!.Name;

            if (!names.Add(serviceName))
            {
                throw new InvalidDataException($"Il catalogo contiene il servizio duplicato {serviceName}.");
            }
        }
    }

    public static void ValidateService(Servizio? service)
    {
        if (service is null ||
            string.IsNullOrWhiteSpace(service.Name) ||
            service.Name.Length > MaximumServiceNameLength ||
            service.Name.Any(char.IsControl) ||
            service.Name.Contains('\'', StringComparison.Ordinal))
        {
            throw new InvalidDataException("Nome servizio non valido nel catalogo remoto.");
        }

        if (string.IsNullOrWhiteSpace(service.StartupType) || !AllowedStartupTypes.Contains(service.StartupType))
        {
            throw new InvalidDataException($"Tipo di avvio non consentito per il servizio {service.Name}.");
        }
    }
}

public sealed class Servizio
{
    public required string Name { get; set; }
    public required string StartupType { get; set; }
    public required string OriginalType { get; set; }
}

public sealed class ServiziRoot
{
    public required List<Servizio> service { get; set; }
}
