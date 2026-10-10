using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class ServiceConfigurationValidatorTests
{
    [Theory]
    [InlineData("Automatic")]
    [InlineData("AutomaticDelayedStart")]
    [InlineData("Manual")]
    [InlineData("Disabled")]
    public void ValidateService_AcceptsSupportedStartupTypes(string startupType)
    {
        ServiceConfigurationValidator.ValidateService(CreateService("BITS", startupType));
    }

    [Theory]
    [InlineData("AutomaticDelayedStart; Start-Process calc")]
    [InlineData("")]
    public void ValidateService_RejectsUnsupportedStartupTypes(string startupType)
    {
        Assert.Throws<InvalidDataException>(() =>
            ServiceConfigurationValidator.ValidateService(CreateService("BITS", startupType)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Bad'Name")]
    [InlineData("Bad\nName")]
    public void ValidateService_RejectsMalformedNames(string name)
    {
        Assert.Throws<InvalidDataException>(() =>
            ServiceConfigurationValidator.ValidateService(CreateService(name, "Manual")));
    }

    [Fact]
    public void ValidateCatalog_RejectsDuplicateServiceNamesIgnoringCase()
    {
        var catalog = new ServiziRoot
        {
            service = [CreateService("BITS", "Manual"), CreateService("bits", "Disabled")]
        };

        Assert.Throws<InvalidDataException>(() => ServiceConfigurationValidator.ValidateCatalog(catalog));
    }

    private static Servizio CreateService(string name, string startupType) => new()
    {
        Name = name,
        StartupType = startupType,
        OriginalType = "Manual"
    };
}
