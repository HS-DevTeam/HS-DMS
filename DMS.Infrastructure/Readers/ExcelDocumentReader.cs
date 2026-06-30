using ClosedXML.Excel;
using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using System.Text;

namespace DMS.Infrastructure.Readers;

public sealed class ExcelDocumentReader : IDocumentReader
{
   public Task<DocumentReadResult> ReadAsync(
    UploadedDocument document,
    CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();

    using var stream = new MemoryStream(document.Content);
    using var workbook = new XLWorkbook(stream);

    var sb = new StringBuilder();

    foreach (var worksheet in workbook.Worksheets)
    {
        sb.AppendLine($"=== Sheet: {worksheet.Name} ===");

        var usedRange = worksheet.RangeUsed();
        if (usedRange == null)
            continue;

        foreach (var row in usedRange.RowsUsed())
        {
            foreach (var cell in row.Cells())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var value = cell.GetFormattedString();

                if (!string.IsNullOrWhiteSpace(value))
                    sb.Append(value + "\t");
            }

            sb.AppendLine();
        }

        sb.AppendLine();
    }

    return Task.FromResult(
        new DocumentReadResult(sb.ToString(), null)
    );
}
}