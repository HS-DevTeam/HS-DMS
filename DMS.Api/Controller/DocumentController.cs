using DMS.Api.Contracts;
using DMS.Application.Contracts;
using DMS.Application.Request;
using DMS.Domain.Documents;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Api.Controllers;

/// <summary>
/// Operações de validação documental.
/// </summary>
[ApiController]
[Route("api/documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentValidationService _service;

    public DocumentsController(IDocumentValidationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Valida um documento submetido pelo utilizador.
    /// </summary>
    /// <remarks>
    /// Exemplos de utilização:
    ///
    /// - EmployeeList.xlsx → EmployeeList
    /// - VehicleList.xlsx → VehicleList
    /// - Certidao_Comercial.pdf → CommercialCertificate
    ///
    /// O sistema:
    /// - lê o conteúdo do documento;
    /// - identifica automaticamente o tipo documental;
    /// - calcula um nível de confiança;
    /// - valida contra o tipo esperado.
    /// </remarks>
    /// <param name="request">
    /// Pedido contendo o ficheiro e o tipo documental esperado.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelamento da operação.
    /// </param>
    /// <returns>
    /// Resultado da validação documental.
    /// </returns>
    /// <response code="200">
    /// Documento processado com sucesso.
    /// </response>
    /// <response code="400">
    /// Ficheiro não enviado ou inválido.
    /// </response>
    [HttpPost("validate")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidateDocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Validate(
        [FromForm] ValidateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("File is required.");
        }

        byte[] content;

        await using (var stream = request.File.OpenReadStream())
        using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream, cancellationToken);
            content = memoryStream.ToArray();
        }

        var document = new UploadedDocument(
            request.File.FileName,
            request.File.ContentType,
            content);

        var validationRequest = new DocumentValidationRequest(
            document,
            request.ExpectedType);

        var result = await _service.ValidateAsync(
            validationRequest,
            cancellationToken);

        var response = new ValidateDocumentResponse
        {
            ExpectedType = result.ExpectedType.ToString(),
            DetectedType = result.Analysis.DocumentType.ToString(),
            Confidence = result.Analysis.Confidence,
            Accepted = result.Accepted,
            Reasons = result.Reasons
        };

        return Ok(response);
    }
}