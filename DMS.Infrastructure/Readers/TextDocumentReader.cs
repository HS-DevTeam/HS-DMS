using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using System.Text;

namespace DMS.Infrastructure.Readers;

public sealed class TextDocumentReader : IDocumentReader
{
    public Task<DocumentReaderResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var text = Encoding.UTF8.GetString(document.Content);

        return Task.FromResult(
            new DocumentReaderResult(
                text,
                new Dictionary<string, string>
                {
                    ["Reader"] = "Text",
                    ["FileName"] = document.FileName,
                    ["ContentType"] = document.ContentType
                }));
    }
}