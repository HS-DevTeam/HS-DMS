using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Domain.Documents
{
    public sealed record DocumentDetectionResult(
        DocType DetectedType,
        decimal Confidence,
        IReadOnlyList<(DocType Type, decimal Score)> Rankings
    );
}