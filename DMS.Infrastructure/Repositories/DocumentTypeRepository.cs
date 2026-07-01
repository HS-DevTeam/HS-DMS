using DMS.Application.Contracts;
using DMS.Domain.Entities;

namespace DMS.Infrastructure.Repositories;

public sealed class DocumentTypeRepository
    : IDocumentTypeRepository
{
    private static readonly IReadOnlyCollection<DocumentType> _types =
    [
        new()
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Commercial Certificate",
            TenantId = null,
            Keywords =
            [
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "certidão comercial"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "nif"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "matrícula"
                }
            ]
        },

        new()
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Employee List",
            TenantId = null,
            Keywords =
            [
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "nome"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "cargo"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "salário"
                }
            ]
        },

        new()
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Vehicle List",
            TenantId = null,
            Keywords =
            [
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "matrícula"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "marca"
                },
                new DocumentKeyword
                {
                    Id = Guid.NewGuid(),
                    Value = "modelo"
                }
            ]
        }
    ];

    public Task<IReadOnlyCollection<DocumentType>> GetAvailableTypesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = _types
            .Where(t =>
                t.TenantId is null ||
                t.TenantId == tenantId)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<DocumentType>>(result);
    }
}