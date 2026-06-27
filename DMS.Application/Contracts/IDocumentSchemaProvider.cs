
using DMS.Domain.Documents;

namespace DMS.Application.Contracts
{
    public interface IDocumentSchemaProvider
    {
        IReadOnlyList<DocumentSchema> GetAll();
    }
}