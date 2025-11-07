using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;

namespace Storage.Infrastructure.Adapters.ObjectStorage.Minio;

/// <summary>
/// MinIO implementation of <see cref="IObjectStoragePresigner"/> that issues presigned PUT and GET URLs.
/// </summary>
public sealed class MinioObjectStoragePresigner : IObjectStoragePresigner
{
  private readonly IMinioClient _client;
  private readonly MinioOptions _options;

  public MinioObjectStoragePresigner(IMinioClient client, IOptions<MinioOptions> opt)
  {
    _client = client;
    _options = opt.Value;
  }

  /// <summary>Generates a presigned PUT URL for uploading an object.</summary>
  public async Task<(string url, DateTimeOffset expiresAt)> PresignPutAsync(ObjectKey key, ContentType contentType, TtlSeconds ttl, CancellationToken ct = default)
  {
    var expirySeconds = ttl.Value;

    var args = new PresignedPutObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(key.Value)
      .WithExpiry(expirySeconds);

    var url = await _client.PresignedPutObjectAsync(args).ConfigureAwait(false);

    var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expirySeconds);

    return (url, expiresAt);
  }

  /// <summary>Generates a presigned GET URL for downloading an object.</summary>
  public async Task<(string url, DateTimeOffset expiresAt)> PresignGetAsync(ObjectKey key, TtlSeconds ttl, CancellationToken ct = default)
  {
    var expirySeconds = ttl.Value;

    var args = new PresignedGetObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(key.Value)
      .WithExpiry(expirySeconds);

    var url = await _client.PresignedGetObjectAsync(args).ConfigureAwait(false);

    var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expirySeconds);

    return (url, expiresAt);
  }
}
