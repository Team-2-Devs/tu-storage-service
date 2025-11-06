using Microsoft.AspNetCore.Mvc;
using Storage.Api.Contracts;
using Storage.Application.Ports.Inbound;

namespace Storage.Api.Controllers.Internal;

[ApiController]
[Route("internal/v1/storage")]
public sealed class StorageController : ControllerBase
{
  private IPresignPutUrl _presignPutUrl;

  public StorageController(IPresignPutUrl presignPutUrl)
  {
    _presignPutUrl = presignPutUrl;
  }

  /// <summary>Internal endpoint that generates presigned PUT URLs for object uploads.</summary>
  [HttpPost("presign-put")]
  public async Task<IActionResult> PresignPut(
    [FromBody] PresignPutRequest req,
    CancellationToken ct)
  {
    var result = await _presignPutUrl.HandleAsync(
      new PresignPutUrlCommand(req.Key, req.ContentType, req.TtlSec), ct);

    return result switch
    {
      PresignPutUrlResult.Success s => Ok(new PresignPutResponse(s.Url, s.ExpiresAt)),
      PresignPutUrlResult.Invalid i => UnprocessableEntity(new { errors = i.Errors }),
      _ => Problem("Internal Server Error", statusCode:500)
    };
  }
}
