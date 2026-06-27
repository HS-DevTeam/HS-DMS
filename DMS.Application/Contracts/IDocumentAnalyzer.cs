using DMS.Application.Services;
using DMS.Domain.Documents;

namespace DMS.Application.Contracts;

public interface IDocumentAnalyzer
{
    Task<DocAnalysis> AnalyzeAsync(DocumentReadResult input, CancellationToken cancellationToken = default);
}