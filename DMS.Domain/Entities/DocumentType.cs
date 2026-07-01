namespace DMS.Domain.Entities;

public sealed class DocumentType
{
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal MinimumConfidence { get; set; } = 0.7m;

    public ICollection<DocumentKeyword> Keywords { get; set; }
        = new List<DocumentKeyword>();
}