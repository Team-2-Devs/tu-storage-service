using Microsoft.AspNetCore.Mvc;
using Storage.Api.Contracts;
using Storage.Application.Ports.Inbound;
using Storage.Application.Ports.Inbound.Contracts;

namespace Storage.Api.Controllers.Internal;

[ApiController]
[Route("internal/v1/storage")]
public sealed class StorageController : ControllerBase
{
  private readonly IPresignPutUrl _presignPutUrl;
  private readonly IPresignGetUrl _presignGetUrl;

  public StorageController(IPresignPutUrl presignPutUrl, IPresignGetUrl presignGetUrl)
  {
    _presignPutUrl = presignPutUrl;
    _presignGetUrl = presignGetUrl;
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
      _ => Problem("Internal Server Error", statusCode: 500)
    };
  }

  /// <summary>Internal endpoint that generates presigned GET URLs for object downloads.</summary>
  [HttpPost("presign-get")]
  public async Task<IActionResult> PresignGet([FromBody] PresignGetRequest req, CancellationToken ct)
  {
    var result = await _presignGetUrl.HandleAsync(
      new PresignGetUrlCommand(req.Key, req.TtlSec), ct);

    return result switch
    {
      PresignGetUrlResult.Success s => Ok(new PresignGetResponse(s.Url, s.ExpiresAt)),
      PresignGetUrlResult.Invalid i => UnprocessableEntity(new { errors = i.Errors }),
      _ => Problem("Internal Server Error", statusCode: 500)
    };
  }
}
