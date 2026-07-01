using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using UglyToad.PdfPig;

namespace DMS.Infrastructure.Readers;

public sealed class PdfDocumentReader : IDocumentReader
{
    public Task<DocumentReaderResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(document.Content);

        var text = string.Empty;

        using (var pdf = PdfDocument.Open(stream))
        {
            text = string.Join(
                Environment.NewLine,
                pdf.GetPages()
                    .Select(page => page.Text));
        }

        return Task.FromResult(
            new DocumentReaderResult(
                text,
                new Dictionary<string, string>
                {
                    ["Source"] = "PdfReader"
                }));
    }
}