using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Application.Services
{
    public sealed record DocumentReadResult(
    string Text,
    IReadOnlyDictionary<string, string>? Metadata = null);
}