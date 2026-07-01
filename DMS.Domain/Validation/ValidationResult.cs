using DMS.Domain.Documents;

namespace DMS.Domain.Validation;

public sealed class ValidationResult
{
    public ValidationResult(
        Guid expectedDocumentTypeId,
        DocAnalysis analysis,
        bool accepted,
        IReadOnlyCollection<string>? reasons = null)
    {
        ExpectedDocumentTypeId = expectedDocumentTypeId;
        Analysis = analysis;
        Accepted = accepted;
        Reasons = reasons ?? [];
    }

    public Guid ExpectedDocumentTypeId { get; }

    public DocAnalysis Analysis { get; }

    public bool Accepted { get; }

    public IReadOnlyCollection<string> Reasons { get; }
}