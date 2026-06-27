using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Domain.Documents
{
    public sealed record UploadedDocument(
    string FileName,
    string ContentType,
    byte[] Content);
}