namespace Storage.Application.Ports.Inbound.Contracts;

/// <summary>Result of a presigned GET URL request.</summary>
public abstract record PresignGetUrlResult
{
  /// <summary>Returned when validation fails.</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : PresignGetUrlResult;
  /// <summary>Returned when the presigned URL was successfully generated.</summary>
  public sealed record Success(string Url, DateTimeOffset ExpiresAt) : PresignGetUrlResult;
}
