using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;
using Storage.Infrastructure.Options;

namespace Storage.Infrastructure.ExternalServices.Minio;

public sealed class MinioObjectStoragePresigner : IObjectStoragePresigner
{
  private readonly MinioClient _client;
  private readonly MinioOptions _options;

  public MinioObjectStoragePresigner(MinioClient client, IOptions<MinioOptions> opt)
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
}
