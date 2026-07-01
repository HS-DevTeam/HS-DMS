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

    public Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        var text = _ocr.ExtractText(document.Content, document.ContentType);

        return Task.FromResult(new DocumentReadResult(text));
    }
    
}