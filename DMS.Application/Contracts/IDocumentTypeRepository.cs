using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Domain.Entities;

public interface IDocumentTypeRepository
{
    Task<IReadOnlyCollection<DocumentType>> GetAvailableTypesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}