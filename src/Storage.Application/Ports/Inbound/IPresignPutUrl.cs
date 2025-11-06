using Storage.Application.Ports.Inbound.Contracts;

namespace Storage.Application.Ports.Inbound;

/// <summary>Defines the inbound port for generating presigned PUT URLs.</summary>
public interface IPresignPutUrl
{
  /// <summary>
  /// Handles a request to generate a presigned PUT URL for object upload.
  /// </summary>
  /// <param name="cmd">The validated upload parameters.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>A result indicating success or validation errors.</returns>
  public Task<PresignPutUrlResult> HandleAsync(PresignPutUrlCommand cmd, CancellationToken ct = default);
}
