namespace DMS.Api.Contracts;

/// <summary>
/// Resultado da validação documental.
/// </summary>
public sealed class ValidateDocumentResponse
{
    /// <summary>
    /// Identificador do tipo documental esperado.
    /// </summary>
    public Guid ExpectedDocumentTypeId { get; init; }

    /// <summary>
    /// Nome do tipo documental esperado.
    /// </summary>
    public string ExpectedDocumentTypeName { get; init; } = string.Empty;

    /// <summary>
    /// Identificador do tipo documental detetado.
    /// </summary>
    public Guid? DetectedDocumentTypeId { get; init; }

    /// <summary>
    /// Nome do tipo documental identificado pelo analisador.
    /// </summary>
    public string? DetectedDocumentTypeName { get; init; }

    /// <summary>
    /// Nível de confiança da classificação.
    /// Valor entre 0 e 1.
    /// </summary>
    /// <example>0.95</example>
    public decimal Confidence { get; init; }

    /// <summary>
    /// Indica se o documento foi aceite pela validação.
    /// </summary>
    /// <example>true</example>
    public bool Accepted { get; init; }

    /// <summary>
    /// Keywords encontradas durante a análise.
    /// </summary>
    public IReadOnlyCollection<string> MatchedKeywords { get; init; }
        = Array.Empty<string>();

    /// <summary>
    /// Lista de motivos de rejeição.
    /// Vazia quando o documento é aceite.
    /// </summary>
    public IReadOnlyCollection<string> Reasons { get; init; }
        = Array.Empty<string>();
}