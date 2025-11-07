namespace Storage.Api.Contracts;

/// <summary>Request for generating a presigned GET URL.</summary>
public sealed record PresignGetRequest(string Key, int TtlSec);
