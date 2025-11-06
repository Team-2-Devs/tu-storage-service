using Storage.Application.Ports.Inbound;
using Storage.Application.Ports.Inbound.Contracts;
using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;

namespace Storage.Application.UseCases;

public sealed class PresignGetUrl : IPresignGetUrl
{
  private readonly IObjectStoragePresigner _presigner;

  public PresignGetUrl(IObjectStoragePresigner presigner) => _presigner = presigner;

  public async Task<PresignGetUrlResult> HandleAsync(PresignGetUrlCommand cmd, CancellationToken ct = default)
  {
    var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

    // Validate
    if (!ObjectKey.TryParse(cmd.Key, out var key, out var keyError))
      errors["key"] = [keyError!];

    if (!TtlSeconds.TryParse(cmd.TtlSec, out var ttlSec, out var ttlSecError))
      errors["ttlSec"] = [ttlSecError!];

    if (errors.Count > 0)
      return new PresignGetUrlResult.Invalid(errors);

    // If Valid
    var (url, expiresAt) = await _presigner.PresignGetAsync(key!, ttlSec, ct);

    return new PresignGetUrlResult.Success(url, expiresAt);
  }
}
