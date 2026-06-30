namespace DMS.Application.Results
{
    public sealed record DocumentReadResult(
    string Text,
    IReadOnlyDictionary<string, string>? Metadata = null);
}