using DMS.Domain.Documents;

namespace DMS.Api.Contracts;

/// <summary>
/// Pedido de validação documental.
/// </summary>
public sealed class ValidateDocumentRequest
{
    /// <summary>
    /// Ficheiro a validar.
    /// </summary>
    /// <remarks>
    /// Formatos suportados:
    /// - PDF
    /// - XLSX
    /// - CSV (futuro)
    /// - Imagens com OCR (futuro)
    /// </remarks>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Tipo documental esperado.
    /// </summary>
    /// <example>CommercialCertificate</example>
    public DocType ExpectedType { get; set; }
}