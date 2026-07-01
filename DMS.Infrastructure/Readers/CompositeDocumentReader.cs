using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;

namespace DMS.Infrastructure.Readers;

public sealed class CompositeDocumentReader : IDocumentReader
{
    private readonly TextDocumentReader _textReader;
    private readonly ExcelDocumentReader _excelReader;
    private readonly PdfDocumentReader _pdfReader;
    //private readonly OcrDocumentReader _ocrReader;

    public CompositeDocumentReader(
        TextDocumentReader textReader,
        ExcelDocumentReader excelReader,
        PdfDocumentReader pdfReader)
        // OcrDocumentReader ocrReader)
    {
        _textReader = textReader;
        _excelReader = excelReader;
        _pdfReader = pdfReader;
        // _ocrReader = ocrReader;
    }

    private static bool IsPdf(byte[] file)
    {
        return file.Length > 4 &&
            file[0] == 0x25 && // %
            file[1] == 0x50 && // P
            file[2] == 0x44 && // D
            file[3] == 0x46;   // F
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

            // ✅ NOVO: imagens vão para OCR via PDF reader ou OCR service
            ".png" or ".jpg" or ".jpeg" or ".tiff" =>
                _pdfReader.ReadAsync(document, cancellationToken),

            _ => throw new NotSupportedException(
                $"Extension '{extension}' is not supported.")
        };
    }

}