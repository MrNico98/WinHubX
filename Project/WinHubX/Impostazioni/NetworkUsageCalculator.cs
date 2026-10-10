namespace WinHubX.Impostazioni;

public static class NetworkUsageCalculator
{
    public static double CalculateCapacityKilobytesPerSecond(IEnumerable<long> linkSpeedsBitsPerSecond)
    {
        ArgumentNullException.ThrowIfNull(linkSpeedsBitsPerSecond);

        double totalBitsPerSecond = 0;
        foreach (long linkSpeed in linkSpeedsBitsPerSecond.Where(static speed => speed > 0))
        {
            totalBitsPerSecond += linkSpeed;
        }

        return double.IsFinite(totalBitsPerSecond)
            ? totalBitsPerSecond / 8d / 1024d
            : 0;
    }

    public static double CalculateUsagePercentage(double throughputKilobytesPerSecond, double capacityKilobytesPerSecond)
    {
        if (!double.IsFinite(throughputKilobytesPerSecond) ||
            !double.IsFinite(capacityKilobytesPerSecond) ||
            capacityKilobytesPerSecond <= 0)
        {
            return 0;
        }

        return Math.Clamp(throughputKilobytesPerSecond / capacityKilobytesPerSecond * 100, 0, 100);
    }
}
