using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Domain.Documents
{
   public sealed class DocumentSchema
    {
        public DocType Type { get; set; }

        public required string[] RequiredColumns { get; set; }

        public string[] OptionalColumns { get; set; } = [];
    }
}