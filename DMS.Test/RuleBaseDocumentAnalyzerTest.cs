using DMS.Application.Results;
using DMS.Domain.Documents;
using DMS.Infrastructure.Analysis;

namespace DMS.Tests.Infrastructure;

public sealed class RuleBasedDocumentAnalyzerTests
{
    private readonly RuleBasedDocumentAnalyzer _analyzer = new();

    [Theory]
    [InlineData("NOME do funcionário | CARGO: Diretor | SALÁRIO bruto", DocType.EmployeeList)]
    [InlineData("Veículo com MATRÍCULA AA-00-BB, MARCA Ford, MODELO Fiesta", DocType.VehicleList)]
    [InlineData("CERTIDÃO COMERCIAL da CONSERVATÓRIA do registo comercial. NIF 500000000.", DocType.CommercialCertificate)]
    public async Task AnalyzeAsync_ShouldDetectCorrectType_WhenTextContainsKeywords(string rawText, DocType expectedType)
    {
        // Arrange
        var input = new DocumentReadResult(rawText);

        // Act
        var result = await _analyzer.AnalyzeAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(expectedType, result.DocumentType);
        Assert.True(result.Confidence > 0m, "A confiança deveria ser maior que zero.");
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldReturnUnknown_WhenTextHasNoMatchingKeywords()
    {
        // Arrange
        var input = new DocumentReadResult("Texto completamente aleatório sobre culinária ou futebol.");

        // Act
        var result = await _analyzer.AnalyzeAsync(input, CancellationToken.None);

        // Assert
        Assert.Equal(DocType.Unknown, result.DocumentType);
        Assert.Equal(0m, result.Confidence);
    }
}