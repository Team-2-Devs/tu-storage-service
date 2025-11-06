using Storage.Application.Ports.Inbound.Contracts;

namespace Storage.Application.Ports.Inbound;

public interface IPresignGetUrl
{
  public Task<PresignGetUrlResult> HandleAsync(PresignGetUrlCommand cmd, CancellationToken ct = default);
}
