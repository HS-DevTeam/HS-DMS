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

    public DocumentsController(
        IDocumentValidationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Valida um documento submetido pelo utilizador.
    /// </summary>
    [HttpPost("validate")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(ValidateDocumentResponse),
        StatusCodes.Status200OK)]
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
        await using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(
                memoryStream,
                cancellationToken);

            content = memoryStream.ToArray();
        }

        var document = new UploadedDocument(
            Guid.NewGuid(),
            request.File.FileName,
            request.File.ContentType,
            content);

        var validationRequest =
            new DocumentValidationRequest(
                request.TenantId,
                document,
                request.ExpectedDocumentTypeId);

        var result = await _service.ValidateAsync(
            validationRequest,
            cancellationToken);

        var response = new ValidateDocumentResponse
        {
            ExpectedDocumentTypeId =
                result.ExpectedDocumentTypeId,

            DetectedDocumentTypeId =
                result.Analysis.DocumentTypeId,

            DetectedDocumentTypeName =
                result.Analysis.DocumentTypeName,

            Confidence =
                result.Analysis.Confidence,

            Accepted =
                result.Accepted,

            MatchedKeywords =
                result.Analysis.MatchedKeywords,

            Reasons =
                result.Reasons
        };

        return Ok(response);
    }
}