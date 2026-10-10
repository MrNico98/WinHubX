using System.Diagnostics;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class NetworkInterfaceRefreshPolicyTests
{
    [Fact]
    public void ShouldRefresh_ReturnsTrueWhenNetworkTopologyChanged()
    {
        Assert.True(NetworkInterfaceRefreshPolicy.ShouldRefresh(
            topologyChanged: true,
            lastRefreshTimestamp: Stopwatch.GetTimestamp(),
            currentTimestamp: Stopwatch.GetTimestamp(),
            refreshInterval: TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void ShouldRefresh_ReturnsTrueWhenRefreshIntervalElapsed()
    {
        long lastRefresh = Stopwatch.GetTimestamp();
        long current = lastRefresh + (long)(TimeSpan.FromMinutes(1).TotalSeconds * Stopwatch.Frequency);

        Assert.True(NetworkInterfaceRefreshPolicy.ShouldRefresh(
            topologyChanged: false,
            lastRefreshTimestamp: lastRefresh,
            currentTimestamp: current,
            refreshInterval: TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void ShouldRefresh_ReturnsFalseBeforeRefreshInterval()
    {
        long lastRefresh = Stopwatch.GetTimestamp();
        long current = lastRefresh + (long)(TimeSpan.FromSeconds(30).TotalSeconds * Stopwatch.Frequency);

        Assert.False(NetworkInterfaceRefreshPolicy.ShouldRefresh(
            topologyChanged: false,
            lastRefreshTimestamp: lastRefresh,
            currentTimestamp: current,
            refreshInterval: TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void ShouldRefresh_ReturnsTrueForMissingOrRewoundTimestamps()
    {
        Assert.True(NetworkInterfaceRefreshPolicy.ShouldRefresh(false, 0, 1, TimeSpan.FromMinutes(1)));
        Assert.True(NetworkInterfaceRefreshPolicy.ShouldRefresh(false, 2, 1, TimeSpan.FromMinutes(1)));
    }
}
