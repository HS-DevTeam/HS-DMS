namespace DMS.Api.Contracts;

/// <summary>
/// Resultado da validação documental.
/// </summary>
public sealed class ValidateDocumentResponse
{
    /// <summary>
    /// Tipo documental esperado pelo consumidor da API.
    /// </summary>
    /// <example>CommercialCertificate</example>
    public string ExpectedType { get; init; } = string.Empty;

    /// <summary>
    /// Tipo documental identificado pelo analisador.
    /// </summary>
    /// <example>CommercialCertificate</example>
    public string DetectedType { get; init; } = string.Empty;

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
    /// Lista de motivos de rejeição.
    /// Vazia quando o documento é aceite.
    /// </summary>
    /// <example>["TYPE_MISMATCH"]</example>
    public IReadOnlyCollection<string> Reasons { get; init; }
        = Array.Empty<string>();
}