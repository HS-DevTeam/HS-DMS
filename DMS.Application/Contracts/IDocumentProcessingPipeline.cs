

using DMS.Domain.Documents;

namespace DMS.Application.Contracts;

public interface IDocumentProcessingPipeline
{
    Task<DocAnalysis> ProcessAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default);
}