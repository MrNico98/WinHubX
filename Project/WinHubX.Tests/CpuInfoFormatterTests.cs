using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class CpuInfoFormatterTests
{
    [Fact]
    public void Format_PreservesSingleProcessorDisplay()
    {
        string result = CpuInfoFormatter.Format([new CpuProcessorInfo("Example CPU", 16)]);

        Assert.Equal("Example CPU (16 thread logici)", result);
    }

    [Fact]
    public void Format_AggregatesLogicalProcessorsAcrossSockets()
    {
        string result = CpuInfoFormatter.Format(
        [
            new CpuProcessorInfo("Example CPU", 16),
            new CpuProcessorInfo("Example CPU", 16)
        ]);

        Assert.Equal("Example CPU (32 thread logici)", result);
    }

    [Fact]
    public void Format_ListsDistinctProcessorModelsAndAggregatesCounts()
    {
        string result = CpuInfoFormatter.Format(
        [
            new CpuProcessorInfo("CPU A", 8),
            new CpuProcessorInfo("CPU B", 12),
            new CpuProcessorInfo("cpu a", 8)
        ]);

        Assert.Equal("CPU A + CPU B (28 thread logici)", result);
    }

    [Fact]
    public void Format_OmitsInvalidNamesAndCounts()
    {
        string result = CpuInfoFormatter.Format(
        [
            new CpuProcessorInfo(" ", 64),
            new CpuProcessorInfo("Example CPU", 0),
            new CpuProcessorInfo("", -1)
        ]);

        Assert.Equal("Example CPU", result);
    }

    [Fact]
    public void Format_ReturnsUnknownWhenNoProcessorNamesAreAvailable()
    {
        Assert.Equal("Sconosciuto", CpuInfoFormatter.Format([]));
    }
}
