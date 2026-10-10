using System.IO.Compression;
using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class SafeZipExtractorTests
{
    [Theory]
    [InlineData("../outside.txt")]
    [InlineData("nested/../../outside.txt")]
    [InlineData("C:/outside.txt")]
    [InlineData("/outside.txt")]
    public void ResolveContainedPath_RejectsRootedAndTraversingPaths(string relativePath)
    {
        using TemporaryDirectory temporary = new();

        Assert.Throws<InvalidDataException>(() => SafePathResolver.ResolveContainedPath(temporary.Path, relativePath));
    }

    [Fact]
    public void ResolveContainedPath_CombinesNestedRelativePathUnderRoot()
    {
        using TemporaryDirectory temporary = new();

        string result = SafePathResolver.ResolveContainedPath(temporary.Path, "Risorse/unattend.xml");

        Assert.Equal(Path.GetFullPath(Path.Join(temporary.Path, "Risorse", "unattend.xml")), result);
    }

    [Fact]
    public void ExtractToFreshDirectory_ExtractsNestedFiles()
    {
        using TemporaryDirectory temporary = new();
        string archivePath = Path.Combine(temporary.Path, "resources.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
        {
            using StreamWriter writer = new(archive.CreateEntry("Risorse/unattend.xml").Open());
            writer.Write("<unattend />");
        }

        string destination = Path.Combine(temporary.Path, "extracted");
        SafeZipExtractor.ExtractToFreshDirectory(archivePath, destination);

        Assert.Equal("<unattend />", File.ReadAllText(Path.Combine(destination, "Risorse", "unattend.xml")));
    }

    [Theory]
    [InlineData("../outside.txt")]
    [InlineData("Risorse/../../outside.txt")]
    [InlineData("C:/outside.txt")]
    [InlineData("/outside.txt")]
    public void ExtractToFreshDirectory_RejectsUnsafeEntryNames(string entryName)
    {
        using TemporaryDirectory temporary = new();
        string archivePath = Path.Combine(temporary.Path, "unsafe.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            archive.CreateEntry(entryName);

        string destination = Path.Combine(temporary.Path, "extracted");
        Assert.Throws<InvalidDataException>(() => SafeZipExtractor.ExtractToFreshDirectory(archivePath, destination));
        Assert.False(File.Exists(Path.Combine(temporary.Path, "outside.txt")));
    }

    [Fact]
    public void ExtractToFreshDirectory_RejectsExistingDestination()
    {
        using TemporaryDirectory temporary = new();
        string archivePath = Path.Combine(temporary.Path, "empty.zip");
        using (ZipFile.Open(archivePath, ZipArchiveMode.Create)) { }
        string destination = Path.Combine(temporary.Path, "existing");
        Directory.CreateDirectory(destination);

        Assert.Throws<IOException>(() => SafeZipExtractor.ExtractToFreshDirectory(archivePath, destination));
    }

    [Fact]
    public void ExtractToFreshDirectory_RejectsSymbolicLinkEntries()
    {
        using TemporaryDirectory temporary = new();
        string archivePath = Path.Combine(temporary.Path, "link.zip");
        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
        {
            ZipArchiveEntry entry = archive.CreateEntry("link");
            entry.ExternalAttributes = unchecked((int)0xA1FF0000);
            using Stream stream = entry.Open();
            stream.Write(Encoding.UTF8.GetBytes("target"));
        }

        Assert.Throws<InvalidDataException>(() =>
            SafeZipExtractor.ExtractToFreshDirectory(archivePath, Path.Combine(temporary.Path, "extracted")));
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        internal string Path { get; }

        internal TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"WinHubX-ZipTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
