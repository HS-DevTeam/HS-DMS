using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Application.Contracts;

public interface IOcrService
{
    string ExtractText(byte[] fileBytes, string contentType);
}