using DMS.Application.Contracts;
using DMS.Application.Request;
using DMS.Application.Results;
using DMS.Application.Services;
using DMS.Domain.Documents;

namespace DMS.Tests.Application;

public sealed class DocumentValidationServiceTests
{
    private readonly IDocumentReader _readerMock;
    private readonly IDocumentAnalyzer _analyzerMock;
    private readonly DocumentValidationService _sut; // SUT = System Under Test

    public DocumentValidationServiceTests()
    {
        // Inicializa os Mocks usando o NSubstitute
        _readerMock = Substitute.For<IDocumentReader>();
        _analyzerMock = Substitute.For<IDocumentAnalyzer>();

        // Injeta os mocks no serviço real
        _sut = new DocumentValidationService(_readerMock, _analyzerMock);
    }

    [Fact]
    public async Task ValidateAsync_ShouldAcceptDocument_WhenTypeMatchesAndConfidenceIsHigh()
    {
        // Arrange (Configuração do cenário)
        var document = new UploadedDocument("lista_empregados.xlsx", "application/vnd.ms-excel", Array.Empty<byte>());
        var request = new DocumentValidationRequest(document, DocType.EmployeeList);

        var readResult = new DocumentReadResult("Texto extraído com Nome, Cargo, Salario");
        var analysisResult = new DocAnalysis 
        { 
            DocumentType = DocType.EmployeeList, 
            Confidence = 0.90m // 90% de confiança (maior que o limite de 85%)
        };

        // Configura o comportamento dos Mocks
        _readerMock.ReadAsync(document, Arg.Any<CancellationToken>()).Returns(readResult);
        _analyzerMock.AnalyzeAsync(readResult, Arg.Any<CancellationToken>()).Returns(analysisResult);

        // Act (Execução da ação)
        var result = await _sut.ValidateAsync(request, CancellationToken.None);

        // Assert (Verificação dos resultados)
        Assert.True(result.Accepted);
        Assert.Empty(result.Reasons);
        Assert.Equal(DocType.EmployeeList, result.Analysis.DocumentType);
        Assert.Equal(0.90m, result.Analysis.Confidence);
    }

    [Fact]
    public async Task ValidateAsync_ShouldRejectDocument_WhenConfidenceIsBelow85Percent()
    {
        // Arrange
        var document = new UploadedDocument("documento_duvidoso.pdf", "application/pdf", Array.Empty<byte>());
        var request = new DocumentValidationRequest(document, DocType.VehicleList);

        var readResult = new DocumentReadResult("Matricula Marca Modelo");
        var analysisResult = new DocAnalysis 
        { 
            DocumentType = DocType.VehicleList, 
            Confidence = 0.80m // 80% é menor que o threshold de 85% do seu serviço
        };

        _readerMock.ReadAsync(document, Arg.Any<CancellationToken>()).Returns(readResult);
        _analyzerMock.AnalyzeAsync(readResult, Arg.Any<CancellationToken>()).Returns(analysisResult);

        // Act
        var result = await _sut.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.Accepted);
        Assert.Contains("Erro de Validação de Documento", result.Reasons);
        // O seu código do serviço zera a confiança no resultado devolvido caso seja rejeitado por tipo, 
        // mas mantém se for o mesmo tipo. Vamos validar conforme a sua regra:
        // Antes: Assert.Equal(0m, result.Analysis.Confidence);
        // CORREÇÃO:
        Assert.Equal(0.80m, result.Analysis.Confidence);
    }

    [Fact]
    public async Task ValidateAsync_ShouldRejectDocument_WhenDetectedTypeDoesNotMatchExpected()
    {
        // Arrange
        var document = new UploadedDocument("certidao.pdf", "application/pdf", Array.Empty<byte>());
        // Esperamos lista de veículos, mas o analisador vai detetar uma Certidão Comercial
        var request = new DocumentValidationRequest(document, DocType.VehicleList);

        var readResult = new DocumentReadResult("Certidao Regulamentar Conservatoria");
        var analysisResult = new DocAnalysis 
        { 
            DocumentType = DocType.CommercialCertificate, 
            Confidence = 0.95m 
        };

        _readerMock.ReadAsync(document, Arg.Any<CancellationToken>()).Returns(readResult);
        _analyzerMock.AnalyzeAsync(readResult, Arg.Any<CancellationToken>()).Returns(analysisResult);

        // Act
        var result = await _sut.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.Accepted);
        Assert.Equal(0m, result.Analysis.Confidence); // Garante que foi zerado por divergência de tipo
        Assert.Contains("Erro de Validação de Documento", result.Reasons);
    }
}