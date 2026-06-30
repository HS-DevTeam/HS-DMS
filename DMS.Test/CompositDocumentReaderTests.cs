using DMS.Application.Contracts;
using DMS.Application.Results;
using DMS.Domain.Documents;
using DMS.Infrastructure.Readers;

namespace DMS.Tests.Infrastructure;

public sealed class CompositeDocumentReaderTests
{
    private readonly FakeReader _textReader;
    private readonly FakeReader _excelReader;
    private readonly FakeReader _pdfReader;
    private readonly FakeReader _ocrReader;
    private readonly CompositeDocumentReader _sut;

    public CompositeDocumentReaderTests()
    {
        // Instanciamos objetos reais herdados se possível, mas como são sealed, 
        // vamos passar instâncias REAIS das classes de infraestrutura (passando parâmetros nulos/vazios)
        // para servirem de stubs controlados.
        
        _textReader = new FakeReader();
        _excelReader = new FakeReader();
        _pdfReader = new FakeReader();
        _ocrReader = new FakeReader();

        // Para contornar o sealed sem quebrar o construtor, usamos instâncias reais
        // criadas via reflexão ou omitindo comportamentos pesados.
        // No entanto, para testar puramente o mapeamento de extensões, mudamos o construtor 
        // do seu CompositeDocumentReader para interfaces ou removemos temporariamente o sealed dele.
    }

    // Como as suas classes concretas são sealed, o truque definitivo mais limpo para testes 
    // sem alterar produção é extrair interfaces. 
}

// Stub auxiliar para testes se o leitor implementasse interfaces:
public class FakeReader : IDocumentReader
{
    public bool Called { get; private set; }
    public Task<DocumentReadResult> ReadAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        Called = true;
        return Task.FromResult(new DocumentReadResult(string.Empty));
    }
}