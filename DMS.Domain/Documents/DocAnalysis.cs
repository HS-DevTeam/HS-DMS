using System.Collections.ObjectModel;

namespace DMS.Domain.Documents;

public sealed class DocAnalysis
{
    public DocType DocumentType { get; init; }

    private decimal _confidence;
    public decimal Confidence
    {
        get => _confidence;
        init => _confidence = value is >= 0 and <= 1
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Confidence));
    }

    public IReadOnlyDictionary<string, string> Metadata { get; init; }
        = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>());
}