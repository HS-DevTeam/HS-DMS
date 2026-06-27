using DMS.Domain.Documents;

namespace DMS.Api.Contracts;

public sealed class ValidateDocumentRequest
{
    public IFormFile File { get; set; } = null!;

    public DocType ExpectedType { get; set; }
}