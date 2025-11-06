namespace Storage.Application.Ports.Inbound.Contracts;

/// <summary>Result of a presigned PUT URL request.</summary>
public abstract record PresignPutUrlResult
{
  /// <summary>Returned when validation fails.</summary>
  public sealed record Invalid(Dictionary<string, string[]> Errors) : PresignPutUrlResult;
  /// <summary>Returned when the presigned URL was successfully generated.</summary>
  public sealed record Success(string Url, DateTimeOffset ExpiresAt) : PresignPutUrlResult;
}
