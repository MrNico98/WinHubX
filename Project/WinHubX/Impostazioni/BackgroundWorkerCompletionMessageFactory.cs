using System.ComponentModel;

namespace WinHubX.Impostazioni;

public enum CompletionMessageSeverity
{
    Information,
    Warning,
    Error
}

public sealed record CompletionMessage(string Text, CompletionMessageSeverity Severity);

public static class BackgroundWorkerCompletionMessageFactory
{
    public static CompletionMessage Create(
        RunWorkerCompletedEventArgs eventArgs,
        string partialFailureTitle,
        string successMessage)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);
        ArgumentException.ThrowIfNullOrWhiteSpace(partialFailureTitle);
        ArgumentNullException.ThrowIfNull(successMessage);

        if (eventArgs.Error is not null)
        {
            return new CompletionMessage(
                $"Operazione non completata: {eventArgs.Error.GetBaseException().Message}",
                CompletionMessageSeverity.Error);
        }

        if (eventArgs.Cancelled)
            return new CompletionMessage("Operazione annullata.", CompletionMessageSeverity.Warning);

        if (eventArgs.Result is IReadOnlyCollection<string> failures && failures.Count > 0)
        {
            string[] distinctFailures = failures.Distinct(StringComparer.Ordinal).ToArray();
            string details = string.Join(Environment.NewLine, distinctFailures.Take(5));
            string remaining = distinctFailures.Length > 5
                ? $"{Environment.NewLine}Altri errori: {distinctFailures.Length - 5}."
                : string.Empty;

            return new CompletionMessage(
                $"{partialFailureTitle}{Environment.NewLine}{details}{remaining}",
                CompletionMessageSeverity.Warning);
        }

        return new CompletionMessage(successMessage, CompletionMessageSeverity.Information);
    }
}
