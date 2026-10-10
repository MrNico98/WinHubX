using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class HardwareSensorReadingTests
{
    [Fact]
    public void SelectTemperature_SkipsUnavailableAndOutOfRangeReadings()
    {
        float? result = HardwareSensorReading.SelectTemperature([null, 0f, -1f, float.NaN, float.PositiveInfinity, 151f, 48.5f]);

        Assert.Equal(48.5f, result);
    }

    [Fact]
    public void SelectTemperature_ReturnsNullWhenNoSensorHasAUsableValue()
    {
        float? result = HardwareSensorReading.SelectTemperature([null, 0f, -2f, float.NaN, float.NegativeInfinity, 151f]);

        Assert.Null(result);
    }

    [Fact]
    public void SelectTemperature_PreservesReadingAtUpperBoundary()
    {
        float? result = HardwareSensorReading.SelectTemperature([150f]);

        Assert.Equal(150f, result);
    }

    [Fact]
    public void SelectPercentage_ClampsValuesAboveOneHundred()
    {
        float? result = HardwareSensorReading.SelectPercentage([104f]);

        Assert.Equal(100f, result);
    }

    [Fact]
    public void SelectPercentage_SkipsInvalidReadingsAndPreservesValidValues()
    {
        float? result = HardwareSensorReading.SelectPercentage([null, float.NaN, -1f, 72.5f]);

        Assert.Equal(72.5f, result);
    }
}
