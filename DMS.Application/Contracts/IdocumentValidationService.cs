using DMS.Application.Request;
using DMS.Domain.Validation;

namespace DMS.Application.Contracts;

public interface IDocumentValidationService
{
    Task<ValidationResult> ValidateAsync(
        DocumentValidationRequest request,
        CancellationToken cancellationToken = default);
}