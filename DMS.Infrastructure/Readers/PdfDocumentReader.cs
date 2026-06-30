using UglyToad.PdfPig;
using DMS.Application.Contracts;
using DMS.Domain.Documents;
using System.Text;
using DMS.Application.Results;
using DMS.Infrastructure.Readers;

namespace DMS.Infrastructure.Readers;

public sealed class PdfDocumentReader : IDocumentReader
{
    private readonly OcrDocumentReader _ocrReader;

    // Injetamos o leitor de OCR para usar caso o PDF seja um scan sem texto nativo
    public PdfDocumentReader(OcrDocumentReader ocrReader)
    {
        _ocrReader = ocrReader;
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

        // Se o PDF tiver texto nativo, devolvemos imediatamente (Performance ideal)
        if (!string.IsNullOrWhiteSpace(extractedText.Replace("\r", "").Replace("\n", "").Trim()))
        {
            return new DocumentReadResult(extractedText, null);
        }

        // Se o texto veio vazio, é um PDF escaneado. Ativamos o OCR pesado.
        return await _ocrReader.ReadAsync(document, cancellationToken);
    }
}