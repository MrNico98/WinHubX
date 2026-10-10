using System.Runtime.Versioning;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class PrivateUserWorkspaceTests
{
    [Fact]
    [SupportedOSPlatform("windows")]
    public void CreateSession_CreatesPrivateGuidDirectoryAndCanDeleteOnlyThatSession()
    {
        string sessionPath = PrivateUserWorkspace.CreateSession();

        try
        {
            Assert.Equal(sessionPath, PrivateUserWorkspace.ValidateSessionPath(sessionPath));
            Assert.True(Directory.Exists(sessionPath));
        }
        finally
        {
            PrivateUserWorkspace.DeleteSession(sessionPath);
        }

        Assert.False(Directory.Exists(sessionPath));
    }

    [Fact]
    [SupportedOSPlatform("windows")]
    public void ValidateSessionPath_RejectsPathsOutsideWorkspace()
    {
        Assert.Throws<InvalidDataException>(() =>
            PrivateUserWorkspace.ValidateSessionPath(Path.GetTempPath()));
    }
}
