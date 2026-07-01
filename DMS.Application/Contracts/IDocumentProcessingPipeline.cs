

using DMS.Domain.Documents;

namespace DMS.Application.Contracts;
public interface IDocumentProcessingPipeline
{
    Task<DocAnalysis> ProcessAsync(
        Guid tenantId,
        UploadedDocument document,
        CancellationToken cancellationToken = default);
}