using System.Diagnostics;

namespace WinHubX.Impostazioni;

public static class NetworkInterfaceRefreshPolicy
{
    public static bool ShouldRefresh(
        bool topologyChanged,
        long lastRefreshTimestamp,
        long currentTimestamp,
        TimeSpan refreshInterval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(refreshInterval, TimeSpan.Zero);

        if (topologyChanged || lastRefreshTimestamp <= 0 || currentTimestamp < lastRefreshTimestamp)
        {
            return true;
        }

        return Stopwatch.GetElapsedTime(lastRefreshTimestamp, currentTimestamp) >= refreshInterval;
    }
}
