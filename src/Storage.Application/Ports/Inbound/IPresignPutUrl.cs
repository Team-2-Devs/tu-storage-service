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

/// <summary>Command data for requesting a presigned PUT URL.</summary>
public sealed record PresignPutUrlCommand(string Key, string ContentType, int TtlSec);

/// <summary>Result of a presigned PUT URL request.</summary>
public abstract record PresignPutUrlResult
{
  /// <summary>Returned when validation fails.</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : PresignPutUrlResult;
  /// <summary>Returned when the presigned URL was successfully generated.</summary>
  public sealed record Success(string Url, DateTimeOffset ExpiresAt) : PresignPutUrlResult;
}
