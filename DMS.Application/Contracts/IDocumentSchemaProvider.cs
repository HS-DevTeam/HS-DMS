
using DMS.Domain.Documents;
using DMS.Domain.ValueObjects;

namespace DMS.Application.Contracts;

public interface IDocumentSchemaProvider
{
    IReadOnlyList<DocumentSchema> GetAll();
}