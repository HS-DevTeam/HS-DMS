using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Domain.Documents;

namespace DMS.Application.Request
{
    public sealed record DocumentValidationRequest(
        UploadedDocument Document,
        DocType ExpectedType);
}