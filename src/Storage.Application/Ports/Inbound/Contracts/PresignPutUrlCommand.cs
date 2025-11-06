namespace Storage.Application.Ports.Inbound.Contracts;

/// <summary>Command data for requesting a presigned PUT URL.</summary>
public sealed record PresignPutUrlCommand(string Key, string ContentType, int TtlSec);
