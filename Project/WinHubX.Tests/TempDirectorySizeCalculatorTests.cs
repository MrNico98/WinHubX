using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class TempDirectorySizeCalculatorTests
{
    [Fact]
    public void Calculate_SumsFilesInRootAndNestedDirectories()
    {
        string root = Path.Combine(Path.GetTempPath(), $"WinHubX.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, "nested", "deeper"));

        try
        {
            File.WriteAllBytes(Path.Combine(root, "root.bin"), new byte[13]);
            File.WriteAllBytes(Path.Combine(root, "nested", "nested.bin"), new byte[29]);
            File.WriteAllBytes(Path.Combine(root, "nested", "deeper", "deep.bin"), new byte[47]);

            long size = TempDirectorySizeCalculator.Calculate(root, CancellationToken.None);

            Assert.Equal(89, size);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Calculate_ThrowsWhenCancellationWasRequested()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.Throws<OperationCanceledException>(() =>
            TempDirectorySizeCalculator.Calculate(Path.GetTempPath(), cancellation.Token));
    }

    [Fact]
    public void Calculate_RejectsANullPath()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TempDirectorySizeCalculator.Calculate(null!, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Calculate_RejectsAnEmptyPath(string path)
    {
        Assert.Throws<ArgumentException>(() =>
            TempDirectorySizeCalculator.Calculate(path, CancellationToken.None));
    }
}
