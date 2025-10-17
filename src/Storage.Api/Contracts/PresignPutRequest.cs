namespace Storage.Api.Contracts;

public sealed record PresignPutRequest(string Key, string ContentType, int TtlSec);
