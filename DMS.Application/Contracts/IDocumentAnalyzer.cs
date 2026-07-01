using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Application.Contracts;
public interface IDocumentAnalyzer
{
    Task<DocAnalysis> AnalyzeAsync(
        Guid tenantId,
        DocumentReaderResult input,
        CancellationToken cancellationToken = default
    );
}