using Storage.Domain.ValueObjects;

namespace Storage.Application.Ports.Outbound;

/// <summary>
/// Outbound port for generating presigned URLs in an object storage system (e.g., S3/MinIO).
/// Implemented by infrastructure; consumed by application use cases.
/// </summary>
public interface IObjectStoragePresigner
{
  /// <summary>
  /// Creates a presigned PUT URL for uploading an object with a specific content type and TTL.
  /// </summary>
  /// <param name="key">Validated object key.</param>
  /// <param name="contentType">Validated image content type.</param>
  /// <param name="ttl">Validated time-to-live (1..60 seconds).</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>The URL and the absolute expiry timestamp in UTC.</returns>
  public Task<(string url, DateTimeOffset expiresAt)> PresignPutAsync(
    ObjectKey objectKey,
    ContentType contentType,
    TtlSeconds ttl,
    CancellationToken ct = default
    );
}
