using DMS.Infrastructure.OCR;
using Xunit;

namespace DMS.Tests.Ocr;

public class TesseractOcrServiceTests
{
    [Fact]
    public void Should_extract_text_from_image()
    {
        // Arrange
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "test_sample.png");
        var bytes = File.ReadAllBytes(path);

        var ocr = new TesseractOcrService();

        // Act
        var result = ocr.ExtractText(bytes);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Contains("matrícula", result, StringComparison.OrdinalIgnoreCase);
    }
}