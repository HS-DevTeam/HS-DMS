namespace DMS.Domain.Documents;

public sealed record UploadedDocument(
    Guid Id,
    string FileName,
    string ContentType,
    byte[] Content);