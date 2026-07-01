using DMS.Application.Contracts;
using DMS.Application.Request;
using DMS.Domain.Documents;
using DMS.Domain.Validation;

namespace DMS.Application.Services;

public sealed class DocumentValidationService(
    IDocumentReader reader,
    IDocumentAnalyzer analyzer)
    : IDocumentValidationService
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
            request.TenantId,
            content,
            cancellationToken);

        var isSameType =
            analysis.DocumentTypeId ==
            request.ExpectedDocumentTypeId;

        var accepted =
            isSameType &&
            analysis.Confidence >= 0.85m;

        var reasons = accepted
            ? Array.Empty<string>()
            : new[]
            {
                "O documento não corresponde ao tipo esperado."
            };

        return new ValidationResult(
            request.ExpectedDocumentTypeId,
            analysis,
            accepted,
            reasons);
    }
}