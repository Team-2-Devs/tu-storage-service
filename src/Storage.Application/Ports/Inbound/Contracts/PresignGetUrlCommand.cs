namespace Storage.Application.Ports.Inbound.Contracts;

public sealed record PresignGetUrlCommand(string Key, int TtlSec);
