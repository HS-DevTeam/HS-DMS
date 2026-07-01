using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using System.Globalization;
using System.Text;

namespace DMS.Infrastructure.Analysis;

public sealed class RuleBasedDocumentAnalyzer : IDocumentAnalyzer
{
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public RuleBasedDocumentAnalyzer(
        IDocumentTypeRepository documentTypeRepository)
    {
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<DocAnalysis> AnalyzeAsync(
        Guid tenantId,
        DocumentReaderResult input,
        CancellationToken cancellationToken = default)
    {
        var content = RemoveAccents(
            input.Text.ToLowerInvariant());

        var documentTypes =
            await _documentTypeRepository.GetAvailableTypesAsync(
                tenantId,
                cancellationToken);

        var results = documentTypes
            .Select(type =>
            {
                var matchedKeywords = type.Keywords
                    .Where(k =>
                        content.Contains(
                            RemoveAccents(
                                k.Value.ToLowerInvariant())))
                    .Select(k => k.Value)
                    .ToList();

                var confidence =
                    type.Keywords.Count == 0
                        ? 0m
                        : (decimal)matchedKeywords.Count /
                          type.Keywords.Count;

                return new
                {
                    Type = type,
                    Confidence = confidence,
                    MatchedKeywords = matchedKeywords
                };
            })
            .OrderByDescending(x => x.Confidence)
            .FirstOrDefault();

        if (results is null || results.Confidence <= 0)
        {
            return new DocAnalysis
            {
                Confidence = 0,
                MatchedKeywords = Array.Empty<string>(),
                Metadata = input.Metadata ??
                    new Dictionary<string, string>()
            };
        }

        return new DocAnalysis
        {
            DocumentTypeId = results.Type.Id,
            DocumentTypeName = results.Type.Name,
            Confidence = results.Confidence,
            MatchedKeywords = results.MatchedKeywords,
            Metadata = input.Metadata ??
                new Dictionary<string, string>()
        };
    }

    private static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized =
            text.Normalize(NormalizationForm.FormD);

        var builder =
            new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c)
                != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}