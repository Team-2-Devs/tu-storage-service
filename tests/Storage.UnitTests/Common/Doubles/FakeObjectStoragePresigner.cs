using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;

namespace Storage.UnitTests.Common.Doubles;

/// <summary>
/// In-memory fake for <see cref="IObjectStoragePresigner"/> used in unit tests.
/// Records inputs for state verification and returns fixed dummy values.
/// </summary>
/// <remarks>
/// Custom test double; equivalent behavior to a mocking framework (e.g., Moq)
/// </remarks>
public sealed class FakeObjectStoragePresigner : IObjectStoragePresigner
{
  public ObjectKey? LastKey { get; private set; }
  public TtlSeconds? LastTtl { get; private set; }
  public ContentType? LastContentType { get; private set; }
  public string? LastMethod { get; private set; } // "GET" or "PUT"

  public string UrlToReturn { get; set; } = "http://localhost/dummy";
  public DateTimeOffset ExpiresAtToReturn { get; set; } = DateTimeOffset.UtcNow.AddMinutes(5);

  public Task<(string url, DateTimeOffset expiresAt)> PresignGetAsync(
    ObjectKey key, 
    TtlSeconds ttl, 
    CancellationToken ct = default)
  {
    LastKey = key;
    LastTtl = ttl;
    LastMethod = "GET";

    return Task.FromResult((UrlToReturn, ExpiresAtToReturn));
  }

  public Task<(string url, DateTimeOffset expiresAt)> PresignPutAsync(
    ObjectKey key, 
    ContentType contentType, 
    TtlSeconds ttl, 
    CancellationToken ct = default)
  {
    LastKey = key;
    LastTtl = ttl;
    LastContentType = contentType;
    LastMethod = "PUT";

    return Task.FromResult((UrlToReturn, ExpiresAtToReturn));
  }
}
