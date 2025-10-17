namespace Storage.Application.Ports.Inbound;

public interface IPresignPutUrl
{
  public Task<PresignPutUrlResult> HandleAsync(PresignPutUrlCommand cmd, CancellationToken ct = default);
}

public sealed record PresignPutUrlCommand(string Key, string ContentType, int TtlSec);

public abstract record PresignPutUrlResult
{
  public sealed record Invalid(Dictionary<string, string[]> Errors) : PresignPutUrlResult;
  public sealed record Success(string Url, DateTimeOffset ExpiresAt) : PresignPutUrlResult;
}
