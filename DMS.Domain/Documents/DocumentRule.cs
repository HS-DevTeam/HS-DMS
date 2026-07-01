using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Domain.Documents;

public sealed class DocumentRule
{
    public string TenantId { get; init; } = default!;
    public DbType DocumentType { get; init; }
    public decimal MinConfidence { get; init; } = 0.6m;
}