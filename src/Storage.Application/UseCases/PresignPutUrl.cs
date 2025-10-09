using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;

namespace Storage.Application.UseCases;

public sealed class PresignPutUrl
{
  private readonly IObjectStoragePresigner _presigner;

  public PresignPutUrl(IObjectStoragePresigner presigner) => _presigner = presigner;

  public abstract record Outcome
  {
    public sealed record Invalid(Dictionary<string, string[]> Errors) : Outcome;
    public sealed record Success(string Url, DateTimeOffset ExpiresAt) : Outcome;
  };
  public sealed record Command(string Key, string ContentType, int TtlSec);

  public async Task<Outcome> HandleAsync(Command cmd, CancellationToken ct = default)
  {
    var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

    // Validate
    if (!ObjectKey.TryParse(cmd.Key, out var key, out var keyError))
      errors["key"] = [keyError!];

    if (!ContentType.TryParse(cmd.ContentType, out var type, out var typeError))
      errors["contentType"] = [typeError!];

    if (!TtlSeconds.TryParse(cmd.TtlSec, out var ttl, out var ttlError))
      errors["ttlSec"] = [ttlError!];

    if (errors.Count > 0)
      return new Outcome.Invalid(errors);

    // If Valid
    var (url, expiresAt) = await _presigner.PresignPutAsync(key!, type!, ttl, ct);
    
    return new Outcome.Success(url, expiresAt);
  }

}
