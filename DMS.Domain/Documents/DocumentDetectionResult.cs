namespace DMS.Domain.Documents;

public sealed record DocumentDetectionResult(
    Guid? DetectedTypeId,
    string? DetectedTypeName,
    decimal Confidence,
    IReadOnlyList<DocumentDetectionRanking> Rankings
);