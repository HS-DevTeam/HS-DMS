using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Application.Services;

public sealed class DocumentProcessingPipeline : IDocumentProcessingPipeline
{
    private readonly IDocumentReader _reader;
    private readonly IOcrService _ocr;
    private readonly IDocumentAnalyzer _analyzer;

    public DocumentProcessingPipeline(
        IDocumentReader reader,
        IOcrService ocr,
        IDocumentAnalyzer analyzer)
    {
        _reader = reader;
        _ocr = ocr;
        _analyzer = analyzer;
    }

    public async Task<DocAnalysis> ProcessAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        // 1. leitura inicial
        var readResult = await _reader.ReadAsync(document, cancellationToken);

        var text = readResult.Text;

        // 2. fallback OCR
        if (string.IsNullOrWhiteSpace(text))
        {
            text = _ocr.ExtractText(document.Content, document.ContentType);
        }

        // 3. normalizar input para analyzer
        var normalized = new DocumentReadResult(text);

        // 4. análise final
        return await _analyzer.AnalyzeAsync(normalized, cancellationToken);
    }
}