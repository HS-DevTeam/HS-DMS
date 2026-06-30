using DMS.Application.Results;
using DMS.Domain.Documents;
namespace DMS.Application.Contracts;

public interface IDocumentReader
{
    Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default);
}