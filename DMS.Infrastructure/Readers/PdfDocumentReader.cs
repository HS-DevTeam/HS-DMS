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
        if (!IsPdf(document.Content))
        {
            throw new InvalidOperationException(
                $"O ficheiro '{document.FileName}' não é um PDF válido.");
        }

        try
        {
            using var stream = new MemoryStream(document.Content);
            using var pdf = PdfDocument.Open(stream);

            var sb = new StringBuilder();

            foreach (var page in pdf.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();
                sb.AppendLine(page.Text);
            }

            var extractedText = sb.ToString().Trim();

            // PDF com texto pesquisável
            if (HasUsefulText(extractedText))
            {
                return new DocumentReadResult(extractedText);
            }
        }
        catch (Exception)
        {
            // PDF corrompido, protegido ou não suportado
            // tenta OCR antes de falhar
        }

        // PDF escaneado ou sem texto útil
        var ocrText = _ocr.ExtractText(
            document.Content,
            document.ContentType);

        return new DocumentReadResult(ocrText);
    }

    private static bool IsPdf(byte[] file)
    {
        return file.Length > 4 &&
               file[0] == 0x25 && // %
               file[1] == 0x50 && // P
               file[2] == 0x44 && // D
               file[3] == 0x46;   // F
    }

    private static bool HasUsefulText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return text.Count(char.IsLetterOrDigit) > 20;
    }
}