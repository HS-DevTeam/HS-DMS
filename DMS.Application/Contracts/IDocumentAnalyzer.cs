using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Application.Contracts;

public interface IDocumentAnalyzer
{
    Task<DocAnalysis> AnalyzeAsync(DocumentReadResult input, CancellationToken cancellationToken = default);
}