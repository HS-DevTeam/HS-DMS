using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using ImageMagick;
using Tesseract;

namespace DMS.Infrastructure.Readers;

public sealed class OcrDocumentReader : IDocumentReader
{
    private readonly string _tessDataPath;
    private readonly string _language;

    public OcrDocumentReader(string tessDataPath = "./tessdata", string language = "por")
    {
        _tessDataPath = tessDataPath;
        _language = language; // "por" para Português, "eng" para Inglês
    }

    public async Task<DocumentReadResult> ReadAsync(
        UploadedDocument document,
        CancellationToken cancellationToken = default)
    {
        // Se for um PDF, precisamos de processar página a página convertendo em imagem
        if (document.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            var pdfText = await ExtractTextFromPdfAsync(document.Content, cancellationToken);
            return new DocumentReadResult(pdfText);
        }

        // Se for uma imagem direta (PNG, JPEG, TIFF)
        var imageText = await ExtractTextFromImageAsync(document.Content, cancellationToken);
        return new DocumentReadResult(imageText);
    }

    private async Task<string> ExtractTextFromImageAsync(byte[] imageBytes, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            using var engine = new TesseractEngine(_tessDataPath, _language, EngineMode.Default);
            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img);
            
            return page.GetText() ?? string.Empty;
        }, cancellationToken);
    }

    private async Task<string> ExtractTextFromPdfAsync(byte[] pdfBytes, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var fullText = new List<string>();

            // Configurações de leitura do PDF via Magick.NET
            var settings = new MagickReadSettings
            {
                Density = new Density(300, 300) // 300 DPI é o ideal para OCR
            };

            using var images = new MagickImageCollection();
            images.Read(pdfBytes, settings);

            using var engine = new TesseractEngine(_tessDataPath, _language, EngineMode.Default);

            foreach (var image in images)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Converte o frame do PDF para o formato que o Tesseract entende
                image.Format = MagickFormat.Png;
                byte[] pngBytes = image.ToByteArray();

                using var pix = Pix.LoadFromMemory(pngBytes);
                using var page = engine.Process(pix);
                
                var text = page.GetText();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fullText.Add(text);
                }
            }

            return string.Join(Environment.NewLine, fullText);
        }, cancellationToken);
    }
}