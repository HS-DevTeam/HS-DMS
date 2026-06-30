namespace DMS.Domain.Documents
{
    public sealed record UploadedDocument(
    string FileName,
    string ContentType,
    byte[] Content);
}