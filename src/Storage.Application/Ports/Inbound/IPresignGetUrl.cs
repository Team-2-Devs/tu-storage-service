using Storage.Application.Ports.Inbound.Contracts;

namespace Storage.Application.Ports.Inbound;

/// <summary>Defines the inbound port for generating presigned GET URLs.</summary>
public interface IPresignGetUrl
{
  /// <summary>
  /// Handles a request to generate a presigned GET URL for object download.
  /// </summary>
  /// <param name="cmd">The validated download parameters.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>A result indicating success or validation errors.</returns>
  public Task<PresignGetUrlResult> HandleAsync(PresignGetUrlCommand cmd, CancellationToken ct = default);
}
