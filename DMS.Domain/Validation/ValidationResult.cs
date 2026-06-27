using DMS.Domain.Documents;

namespace DMS.Domain.Validation;

public sealed class ValidationResult
{
    public ValidationResult(
        DocType expectedType,
        DocAnalysis analysis,
        bool accepted,
        IReadOnlyCollection<string>? reasons = null)
    {
        ExpectedType = expectedType;
        Analysis = analysis;
        Accepted = accepted;
        Reasons = reasons ?? [];
    }

    public DocType ExpectedType { get; }
    public DocAnalysis Analysis { get; }
    public bool Accepted { get; }
    public IReadOnlyCollection<string> Reasons { get; }
}

