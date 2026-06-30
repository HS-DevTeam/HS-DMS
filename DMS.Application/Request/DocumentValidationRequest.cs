using DMS.Domain.Documents;

namespace DMS.Application.Request
{
    public sealed record DocumentValidationRequest(
        UploadedDocument Document,
        DocType ExpectedType);
}