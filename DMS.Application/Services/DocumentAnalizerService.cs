using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Application.Services;

public sealed class DocumentAnalyzerService : IDocumentAnalyzer
{
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public DocumentAnalyzerService(
        IDocumentTypeRepository documentTypeRepository)
    {
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<DocAnalysis> AnalyzeAsync(
        Guid tenantId,
        DocumentReaderResult input,
        CancellationToken cancellationToken = default)
    {
        var documentTypes =
            await _documentTypeRepository.GetAvailableTypesAsync(
                tenantId,
                cancellationToken);

        var text = input.Text.ToLowerInvariant();

        var analysisResults = documentTypes
            .Select(documentType =>
            {
                var matchedKeywords = documentType.Keywords
                    .Where(keyword =>
                        text.Contains(
                            keyword.Value.ToLowerInvariant()))
                    .Select(keyword => keyword.Value)
                    .ToList();

                var confidence =
                    documentType.Keywords.Count == 0
                        ? 0m
                        : (decimal)matchedKeywords.Count /
                          documentType.Keywords.Count;

                return new
                {
                    DocumentType = documentType,
                    Confidence = confidence,
                    MatchedKeywords = matchedKeywords
                };
            })
            .OrderByDescending(x => x.Confidence)
            .FirstOrDefault();

        if (analysisResults is null || analysisResults.Confidence <= 0)
        {
            return new DocAnalysis
            {
                Confidence = 0,
                MatchedKeywords = Array.Empty<string>(),
                Metadata = input.Metadata
                    ?? new Dictionary<string, string>()
            };
        }

        return new DocAnalysis
        {
            DocumentTypeId = analysisResults.DocumentType.Id,
            DocumentTypeName = analysisResults.DocumentType.Name,
            Confidence = analysisResults.Confidence,
            MatchedKeywords = analysisResults.MatchedKeywords,
            Metadata = input.Metadata
                ?? new Dictionary<string, string>()
        };
    }
}