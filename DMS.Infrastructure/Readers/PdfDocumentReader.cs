using UglyToad.PdfPig;
using DMS.Application.Contracts;
using DMS.Domain.Documents;
using DMS.Application.Results;
using System.Text;

namespace DMS.Infrastructure.Readers;

public sealed class PdfDocumentReader : IDocumentReader
{
    private readonly IOcrService _ocr;

    public PdfDocumentReader(IOcrService ocr)
    {
        _ocr = ocr;
    }

    public async Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(document.Content);
        using var pdf = PdfDocument.Open(stream);

        var sb = new StringBuilder();

        foreach (var page in pdf.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();
            sb.AppendLine(page.Text);
        }

        var extractedText = sb.ToString();

        // PDF com texto normal
        if (!string.IsNullOrWhiteSpace(extractedText.Trim()))
        {
            return new DocumentReadResult(extractedText);
        }

        // PDF escaneado → OCR
        var ocrText = _ocr.ExtractText(document.Content, document.ContentType);

        return new DocumentReadResult(ocrText);
    }
}