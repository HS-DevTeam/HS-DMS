using DMS.Application.Results;
using DMS.Domain.Documents;
namespace DMS.Application.Contracts;

public interface IDocumentReader
{
    Task<DocumentReaderResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default);
}