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
        Guid tenantId,
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        // 1. Leitura inicial
        var readResult = await _reader.ReadAsync(
            document,
            cancellationToken);

        var text = readResult.Text;
    
        // 2. Fallback OCR
        if (string.IsNullOrWhiteSpace(text))
        {
            text = await _ocr.ExtractTextAsync(
                document.Content,
                document.ContentType,
                cancellationToken);
        }

        // 3. Normalizar para o Analyzer
        var normalized = new DocumentReaderResult(
            text,
            readResult.Metadata);

        // 4. Análise final
        return await _analyzer.AnalyzeAsync(
            tenantId,
            normalized,
            cancellationToken);
    }
}