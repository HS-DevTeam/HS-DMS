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
    /// - CSV
    /// - PNG
    /// - JPG
    /// - JPEG
    /// - TIFF
    /// </remarks>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Tenant que está a submeter o documento.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Tipo documental esperado.
    /// </summary>
    public Guid ExpectedDocumentTypeId { get; set; }
}