using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Readers;

public sealed class CompositeDocumentReader : IDocumentReader
{
    private readonly TextDocumentReader _textReader;
    private readonly ExcelDocumentReader _excelReader;
    private readonly PdfDocumentReader _pdfReader;

    public CompositeDocumentReader(
        TextDocumentReader textReader,
        ExcelDocumentReader excelReader,
        PdfDocumentReader pdfReader)
    {
        _textReader = textReader;
        _excelReader = excelReader;
        _pdfReader = pdfReader;
    }

    public Task<DocumentReaderResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        var extension = Path
            .GetExtension(document.FileName)
            .ToLowerInvariant();

        return extension switch
        {
            ".txt" =>
                _textReader.ReadAsync(
                    document,
                    cancellationToken),

            ".xlsx" or ".xls" =>
                _excelReader.ReadAsync(
                    document,
                    cancellationToken),

            ".pdf" =>
                _pdfReader.ReadAsync(
                    document,
                    cancellationToken),

            // Imagens serão tratadas pelo OCR na pipeline
            ".png" or ".jpg" or ".jpeg" or ".tif" or ".tiff" =>
                Task.FromResult(
                    new DocumentReaderResult(
                        string.Empty,
                        new Dictionary<string, string>
                        {
                            ["RequiresOcr"] = "true"
                        })),

            _ => throw new NotSupportedException(
                $"Extension '{extension}' is not supported.")
        };
    }
}