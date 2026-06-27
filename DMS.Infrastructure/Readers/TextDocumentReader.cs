using DMS.Application.Contracts;
using DMS.Application.Services;
using DMS.Domain.Documents;
using System.Text;

namespace DMS.Infrastructure.Readers;

public sealed class TextDocumentReader : IDocumentReader
{
    public Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        var text = Encoding.UTF8.GetString(document.Content);

        var result = new DocumentReadResult(
            text,
            null);

        return Task.FromResult(result);
    }
}