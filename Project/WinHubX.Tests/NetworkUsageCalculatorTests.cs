using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class NetworkUsageCalculatorTests
{
    [Fact]
    public void CalculateCapacityKilobytesPerSecond_ConvertsAndAggregatesLinkSpeeds()
    {
        double capacity = NetworkUsageCalculator.CalculateCapacityKilobytesPerSecond([1_000_000_000, 1_000_000_000]);

        Assert.Equal(244_140.625, capacity);
    }

    [Fact]
    public void CalculateCapacityKilobytesPerSecond_IgnoresUnavailableLinkSpeeds()
    {
        double capacity = NetworkUsageCalculator.CalculateCapacityKilobytesPerSecond([0, -1, 8_192]);

        Assert.Equal(1, capacity);
    }

    [Fact]
    public void CalculateCapacityKilobytesPerSecond_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => NetworkUsageCalculator.CalculateCapacityKilobytesPerSecond(null!));
    }

    [Theory]
    [InlineData(50, 100, 50)]
    [InlineData(125, 100, 100)]
    [InlineData(-5, 100, 0)]
    [InlineData(50, 0, 0)]
    [InlineData(50, -1, 0)]
    [InlineData(double.NaN, 100, 0)]
    [InlineData(double.PositiveInfinity, 100, 0)]
    [InlineData(50, double.PositiveInfinity, 0)]
    public void CalculateUsagePercentage_ClampsAndRejectsInvalidMeasurements(
        double throughput,
        double capacity,
        double expected)
    {
        Assert.Equal(expected, NetworkUsageCalculator.CalculateUsagePercentage(throughput, capacity));
    }
}
