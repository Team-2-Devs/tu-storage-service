using Storage.Application.Ports.Inbound.Contracts;

namespace Storage.UnitTests.Common.Builders;

/// <summary>Builder for presign-commands used in application layer unit tests.</summary>
public sealed class PresignCommandBuilder
{
  private string _key = "images/2025/11/09/sample.jpg";
  private int _ttl = 3600;
  private string _contentType = "image/jpeg";

  public PresignCommandBuilder WithKey(string key)
  {
    _key = key;
    return this;
  }

  public PresignCommandBuilder WithTtl(int ttl)
  {
    _ttl = ttl;
    return this;
  }

  public PresignCommandBuilder WithContentType(string contentType)
  {
    _contentType = contentType;
    return this;
  }
  
  public PresignGetUrlCommand BuildGet() =>
    new(
      Key: _key, 
      TtlSec: _ttl
    );

  public PresignPutUrlCommand BuildPut() =>
    new(
      Key: _key,
      ContentType: _contentType,
      TtlSec: _ttl
    );
}
