using DMS.Domain.Documents;

namespace DMS.Application.Request;

public sealed record DocumentValidationRequest(
    Guid TenantId,
    UploadedDocument Document,
    Guid ExpectedDocumentTypeId);