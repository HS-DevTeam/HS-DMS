namespace DMS.Application.Results;

public sealed record DocumentReaderResult(
    string Text,
    IReadOnlyDictionary<string, string> Metadata);