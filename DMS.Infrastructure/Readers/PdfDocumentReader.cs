using UglyToad.PdfPig;
using DMS.Application.Contracts;
using DMS.Domain.Documents;
using System.Text;
using DMS.Application.Services;

public sealed class PdfDocumentReader : IDocumentReader
{
    public Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(document.Content);
        using var pdf = PdfDocument.Open(stream);

        var sb = new StringBuilder();

        foreach (var page in pdf.GetPages())
        {
            sb.AppendLine(page.Text);
        }

        return Task.FromResult(
            new DocumentReadResult(sb.ToString(), null));
    }
}