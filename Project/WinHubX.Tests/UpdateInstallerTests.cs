using System.Security.Cryptography;
using System.Text;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class UpdateInstallerTests
{
    [Fact]
    public async Task InstallAndStartAsync_ReplacesOnlyAfterHashAndKeepsNewFileWhenReady()
    {
        using var directory = new TemporaryDirectory();
        string source = directory.CreateFile("update.exe", "new-version");
        string destination = directory.CreateFile("WinHubX.exe", "old-version");
        string expectedHash = Hash("new-version");

        await UpdateInstaller.InstallAndStartAsync(
            source,
            destination,
            expectedHash,
            (path, _) => Task.FromResult(File.ReadAllText(path) == "new-version"),
            TestContext.Current.CancellationToken);

        Assert.Equal("new-version", File.ReadAllText(destination));
        Assert.Empty(Directory.EnumerateFiles(directory.Path, "*.rollback"));
        Assert.Empty(Directory.EnumerateFiles(directory.Path, "*.pending"));
    }

    [Fact]
    public async Task InstallAndStartAsync_RestoresBackupWhenNewProcessIsNotReady()
    {
        using var directory = new TemporaryDirectory();
        string source = directory.CreateFile("update.exe", "new-version");
        string destination = directory.CreateFile("WinHubX.exe", "old-version");

        await Assert.ThrowsAsync<InvalidOperationException>(() => UpdateInstaller.InstallAndStartAsync(
            source,
            destination,
            Hash("new-version"),
            (_, _) => Task.FromResult(false),
            TestContext.Current.CancellationToken));

        Assert.Equal("old-version", File.ReadAllText(destination));
        Assert.Empty(Directory.EnumerateFiles(directory.Path, "*.rollback"));
    }

    [Fact]
    public async Task InstallAndStartAsync_RejectsHashMismatchWithoutChangingInstalledFile()
    {
        using var directory = new TemporaryDirectory();
        string source = directory.CreateFile("update.exe", "tampered-version");
        string destination = directory.CreateFile("WinHubX.exe", "old-version");

        await Assert.ThrowsAsync<InvalidDataException>(() => UpdateInstaller.InstallAndStartAsync(
            source,
            destination,
            Hash("expected-version"),
            (_, _) => Task.FromResult(true),
            TestContext.Current.CancellationToken));

        Assert.Equal("old-version", File.ReadAllText(destination));
        Assert.Empty(Directory.EnumerateFiles(directory.Path, "*.pending"));
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"WinHubX-UpdateTests-{Guid.NewGuid():N}");

        public TemporaryDirectory() => Directory.CreateDirectory(Path);

        public string CreateFile(string name, string contents)
        {
            string path = System.IO.Path.Combine(Path, name);
            File.WriteAllText(path, contents);
            return path;
        }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
