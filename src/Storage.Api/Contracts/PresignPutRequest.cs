namespace Storage.Api.Contracts;

/// <summary>Request payload for generating a presigned PUT URL.</summary>
public sealed record PresignPutRequest(string Key, string ContentType, int TtlSec);
