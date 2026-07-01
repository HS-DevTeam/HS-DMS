using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Readers;

public sealed class OcrDocumentReader : IDocumentReader
{
    private readonly IOcrService _ocr;

    public OcrDocumentReader(IOcrService ocr)
    {
        _ocr = ocr;
    }

    public async Task<DocumentReaderResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var text = await _ocr.ExtractTextAsync(
            document.Content,
            document.ContentType,
            cancellationToken);

        return new DocumentReaderResult(
            text,
            new Dictionary<string, string>
            {
                ["Reader"] = "OCR",
                ["FileName"] = document.FileName,
                ["ContentType"] = document.ContentType
            });
    }
}