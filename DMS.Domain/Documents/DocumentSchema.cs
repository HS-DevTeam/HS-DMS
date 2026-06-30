namespace DMS.Domain.Documents
{
   public sealed class DocumentSchema
    {
        public DocType Type { get; set; }

        public required string[] RequiredColumns { get; set; }

        public string[] OptionalColumns { get; set; } = [];
    }
}