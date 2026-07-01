namespace DMS.Domain.Documents;

public sealed class DocAnalysis
{
    public Guid? DocumentTypeId { get; init; }

    public string? DocumentTypeName { get; init; }

    public decimal Confidence { get; init; }

    public IReadOnlyList<string> MatchedKeywords { get; init; }
        = Array.Empty<string>();

    public IReadOnlyDictionary<string, string> Metadata { get; init; }
        = new Dictionary<string, string>();
}