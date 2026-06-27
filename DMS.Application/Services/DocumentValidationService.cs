using DMS.Application.Contracts;
using DMS.Application.Request;
using DMS.Domain.Documents;
using DMS.Domain.Validation;

namespace DMS.Application.Services;

public sealed class DocumentValidationService(
    IDocumentReader reader,
    IDocumentAnalyzer analyzer) : IDocumentValidationService
{
    private readonly IDocumentReader _reader = reader;
    private readonly IDocumentAnalyzer _analyzer = analyzer;

    public async Task<ValidationResult> ValidateAsync(
        DocumentValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        var content = await _reader.ReadAsync(
            request.Document,
            cancellationToken);

        var analysis = await _analyzer.AnalyzeAsync(
            content,
            cancellationToken);

        var isSameType = analysis.DocumentType == request.ExpectedType;

        var confidence = isSameType
            ? analysis.Confidence
            : 0m;

        var accepted =
            isSameType &&
            confidence >= 0.85m;

        var reasons = accepted
            ? []
            : new[] { "Erro de Validação de Documento" };

        return new ValidationResult(
            request.ExpectedType,
            new DocAnalysis
            {
                DocumentType = analysis.DocumentType,
                Confidence = confidence
            },
            accepted,
            reasons
        );
    }
}