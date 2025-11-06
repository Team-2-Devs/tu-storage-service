namespace Storage.Api.Contracts;

public sealed record PresignGetRequest(string Key, int TtlSec);
