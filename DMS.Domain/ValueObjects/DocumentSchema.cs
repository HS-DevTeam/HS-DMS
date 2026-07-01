namespace DMS.Domain.ValueObjects;

public sealed class DocumentSchema
{
    public required string[] RequiredColumns { get; set; }

    public string[] OptionalColumns { get; set; } = [];
}