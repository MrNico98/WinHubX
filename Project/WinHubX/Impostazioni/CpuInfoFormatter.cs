namespace WinHubX.Impostazioni;

public readonly record struct CpuProcessorInfo(string Name, int LogicalProcessorCount);

public static class CpuInfoFormatter
{
    public static string Format(IEnumerable<CpuProcessorInfo> processors)
    {
        ArgumentNullException.ThrowIfNull(processors);

        var names = new List<string>();
        long logicalProcessorCount = 0;

        foreach (CpuProcessorInfo processor in processors)
        {
            string name = processor.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!names.Contains(name, StringComparer.OrdinalIgnoreCase))
                names.Add(name);

            if (processor.LogicalProcessorCount > 0)
                logicalProcessorCount += processor.LogicalProcessorCount;
        }

        if (names.Count == 0)
            return "Sconosciuto";

        string processorName = string.Join(" + ", names);
        return logicalProcessorCount > 0
            ? $"{processorName} ({logicalProcessorCount} thread logici)"
            : processorName;
    }
}
