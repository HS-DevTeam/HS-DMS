using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Readers;

public sealed class CompositeDocumentReader : IDocumentReader
{
    private readonly TextDocumentReader _textReader;
    private readonly ExcelDocumentReader _excelReader;
    private readonly PdfDocumentReader _pdfReader;
    private readonly OcrDocumentReader _ocrReader;

    public CompositeDocumentReader(
        TextDocumentReader textReader,
        ExcelDocumentReader excelReader,
        PdfDocumentReader pdfReader,
        OcrDocumentReader ocrReader)
    {
        _textReader = textReader;
        _excelReader = excelReader;
        _pdfReader = pdfReader;
        _ocrReader = ocrReader;
    }

    public Task<DocumentReadResult> ReadAsync(
    UploadedDocument document,
    CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(document.FileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => _textReader.ReadAsync(document, cancellationToken),
            ".xlsx" or ".xls" => _excelReader.ReadAsync(document, cancellationToken),
            ".pdf" => _pdfReader.ReadAsync(document, cancellationToken),
            ".jpg" or ".jpeg" or ".png" => _ocrReader.ReadAsync(document, cancellationToken),

            _ => throw new NotSupportedException(
                $"Extension '{extension}' is not supported.")
        };
    }

}