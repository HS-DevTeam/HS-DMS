using DMS.Api.Contracts;
using DMS.Application.Contracts;
using DMS.Application.Request;
using DMS.Domain.Documents;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Api.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentValidationService _service;

    public DocumentsController(IDocumentValidationService service)
    {
        _service = service;
    }

    [HttpPost("validate")]
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