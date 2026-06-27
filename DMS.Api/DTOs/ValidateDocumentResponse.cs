namespace DMS.Api.Contracts;

public sealed class ValidateDocumentResponse
{
    public string ExpectedType { get; init; } = string.Empty;

    public string DetectedType { get; init; } = string.Empty;

    public decimal Confidence { get; init; }

    public bool Accepted { get; init; }

    public IReadOnlyCollection<string> Reasons { get; init; }
        = Array.Empty<string>();
}