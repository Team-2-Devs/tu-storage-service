namespace Storage.Application.Ports.Inbound.Contracts;

/// <summary>Command data for requesting a presigned GET URL.</summary>
public sealed record PresignGetUrlCommand(string Key, int TtlSec);
