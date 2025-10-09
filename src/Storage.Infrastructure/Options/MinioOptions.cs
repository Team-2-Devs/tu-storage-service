using System.ComponentModel.DataAnnotations;

namespace Storage.Infrastructure.Options;

public sealed class MinioOptions
{
  [Required] public required string Endpoint { get; init; } = null!;
  [Required] public required string AccessKey { get; init; } = null!;
  [Required] public required string SecretKey { get; init; } = null!;
  [Required] public required string BucketName { get; init; } = null!;

  public bool UseSsl { get; init; } = true;
  public string Region { get; init; } = "eu-central-1";
  public string? PublicBaseUrl { get; init; }
}
