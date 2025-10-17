using Microsoft.AspNetCore.Mvc;
using Storage.Api.Contracts;
using Storage.Application.Ports.Inbound;

namespace Storage.Api.Controllers.Internal;

[ApiController]
[Route("internal/v1/storage")]
public sealed class StorageController : ControllerBase
{
  /// <summary>Internal endpoint that generates presigned PUT URLs for object uploads.</summary>
  [HttpPost("presign-put")]
  public async Task<IActionResult> PresignPut(
    [FromServices] IPresignPutUrl useCase,
    [FromBody] PresignPutRequest req,
    CancellationToken ct)
  {
    var result = await useCase.HandleAsync(
      new PresignPutUrlCommand(req.Key, req.ContentType, req.TtlSec), ct);

    return result switch
    {
      PresignPutUrlResult.Success s => Ok(new { url = s.Url, expiresAt = s.ExpiresAt }),
      PresignPutUrlResult.Invalid i => UnprocessableEntity(new { errors = i.Errors }),
      _ => Problem("Internal Server Error", statusCode: 500)
    };
  }
}
