using System.ComponentModel;
using WinHubX.Impostazioni;
using Xunit;

namespace WinHubX.Tests;

public sealed class BackgroundWorkerCompletionMessageFactoryTests
{
    private const string PartialFailureTitle = "Alcune impostazioni non sono state applicate:";
    private const string SuccessMessage = "Operazione completata.";

    [Fact]
    public void Create_ReportsBaseExceptionForWorkerFailure()
    {
        var eventArgs = new RunWorkerCompletedEventArgs(
            result: null,
            error: new InvalidOperationException("worker failed", new IOException("root cause")),
            cancelled: false);

        CompletionMessage result = BackgroundWorkerCompletionMessageFactory.Create(
            eventArgs,
            PartialFailureTitle,
            SuccessMessage);

        Assert.Equal(CompletionMessageSeverity.Error, result.Severity);
        Assert.Equal("Operazione non completata: root cause", result.Text);
    }

    [Fact]
    public void Create_ReportsCancellationBeforeReadingResult()
    {
        var eventArgs = new RunWorkerCompletedEventArgs(result: null, error: null, cancelled: true);

        CompletionMessage result = BackgroundWorkerCompletionMessageFactory.Create(
            eventArgs,
            PartialFailureTitle,
            SuccessMessage);

        Assert.Equal(CompletionMessageSeverity.Warning, result.Severity);
        Assert.Equal("Operazione annullata.", result.Text);
    }

    [Fact]
    public void Create_ReportsDistinctPartialFailuresAndAccurateRemainder()
    {
        var eventArgs = new RunWorkerCompletedEventArgs(
            new List<string> { "uno", "uno", "due", "tre", "quattro", "cinque", "sei" },
            error: null,
            cancelled: false);

        CompletionMessage result = BackgroundWorkerCompletionMessageFactory.Create(
            eventArgs,
            PartialFailureTitle,
            SuccessMessage);

        Assert.Equal(CompletionMessageSeverity.Warning, result.Severity);
        Assert.Equal(
            $"{PartialFailureTitle}{Environment.NewLine}uno{Environment.NewLine}due{Environment.NewLine}tre{Environment.NewLine}quattro{Environment.NewLine}cinque{Environment.NewLine}Altri errori: 1.",
            result.Text);
    }

    [Fact]
    public void Create_ReturnsSuccessWhenNoFailuresAreReported()
    {
        var eventArgs = new RunWorkerCompletedEventArgs(result: null, error: null, cancelled: false);

        CompletionMessage result = BackgroundWorkerCompletionMessageFactory.Create(
            eventArgs,
            PartialFailureTitle,
            SuccessMessage);

        Assert.Equal(CompletionMessageSeverity.Information, result.Severity);
        Assert.Equal(SuccessMessage, result.Text);
    }

    [Fact]
    public void Create_RejectsNullEventArgs()
    {
        Assert.Throws<ArgumentNullException>(() => BackgroundWorkerCompletionMessageFactory.Create(
            null!,
            PartialFailureTitle,
            SuccessMessage));
    }
}
