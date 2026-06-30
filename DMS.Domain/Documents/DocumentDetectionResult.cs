namespace DMS.Domain.Documents
{
    public sealed record DocumentDetectionResult(
        DocType DetectedType,
        decimal Confidence,
        IReadOnlyList<(DocType Type, decimal Score)> Rankings
    );
}