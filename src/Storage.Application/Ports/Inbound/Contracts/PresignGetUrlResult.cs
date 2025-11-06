namespace Storage.Application.Ports.Inbound.Contracts;

public abstract record PresignGetUrlResult
{
  public sealed record Success(string Url, DateTimeOffset ExpiresAt) : PresignGetUrlResult;
  public sealed record Invalid(Dictionary<string, string[]> Errors) : PresignGetUrlResult;
}
