using ClosedXML.Excel;
using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using System.Text;

namespace DMS.Infrastructure.Readers;

public sealed class ExcelDocumentReader : IDocumentReader
{
    public Task<DocumentReaderResult> ReadAsync(
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

            if (usedRange is null)
                continue;

            foreach (var row in usedRange.RowsUsed())
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var cell in row.Cells())
                {
                    var value = cell.GetFormattedString();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        sb.Append(value);
                        sb.Append('\t');
                    }
                }

                sb.AppendLine();
            }

            sb.AppendLine();
        }

        return Task.FromResult(
            new DocumentReaderResult(
                sb.ToString(),
                new Dictionary<string, string>
                {
                    ["Reader"] = "Excel",
                    ["FileName"] = document.FileName
                }
            )
        );
    }
}