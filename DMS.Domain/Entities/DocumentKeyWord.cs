namespace DMS.Domain.Entities;

public sealed class DocumentKeyword
{
    public Guid Id { get; set; }

    public Guid DocumentTypeId { get; set; }

    public string Value { get; set; } = string.Empty;

    public bool Required { get; set; }
}