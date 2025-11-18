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
  private readonly IMinioClient _internalClient; // for later
  private readonly IMinioClient _publicClient;
  private readonly MinioOptions _options;

  public MinioObjectStoragePresigner(IMinioClient internalClient, IOptions<MinioOptions> opt)
  {
    _internalClient = internalClient;
    _options = opt.Value;

    // Build a separate client for presigning using PublicBaseUrl host
    var publicUri = new Uri(_options.PublicBaseUrl);

    var builder = new MinioClient()
      .WithEndpoint(publicUri.Host, publicUri.IsDefaultPort ? (publicUri.Scheme == "https" ? 443 : 80) : publicUri.Port)
      .WithCredentials(_options.AccessKey, _options.SecretKey)
      .WithRegion(_options.Region);

    if (_options.UseSsl || publicUri.Scheme == Uri.UriSchemeHttps)
      builder = builder.WithSSL();

    _publicClient = builder.Build();
  }

  /// <summary>Generates a presigned PUT URL for uploading an object.</summary>
  public async Task<(string url, DateTimeOffset expiresAt)> PresignPutAsync(ObjectKey key, ContentType contentType, TtlSeconds ttl, CancellationToken ct = default)
  {
    var expirySeconds = ttl.Value;

    var args = new PresignedPutObjectArgs()
      .WithBucket(_options.BucketName)
      .WithObject(key.Value)
      .WithExpiry(expirySeconds);

    var url = await _publicClient.PresignedPutObjectAsync(args).ConfigureAwait(false);

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

    var url = await _publicClient.PresignedGetObjectAsync(args).ConfigureAwait(false);

    var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expirySeconds);

    return (url, expiresAt);
  }
}
