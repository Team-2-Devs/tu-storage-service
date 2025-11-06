using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;
using Storage.Infrastructure.Adapters.ObjectStorage.Options;

namespace Storage.Infrastructure.Adapters.ObjectStorage.Minio;

public sealed class MinioObjectStoragePresigner : IObjectStoragePresigner
{
  private readonly IMinioClient _client;
  private readonly MinioOptions _options;

  public MinioObjectStoragePresigner(IMinioClient client, IOptions<MinioOptions> opt)
  {
    _client = client;
    _options = opt.Value;
  }

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
