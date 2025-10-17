namespace Storage.Api.Contracts;

/// <summary>Response containing a presigned PUT URL and its expiry time.</summary>
public sealed record PresignPutResponse(string Url, DateTimeOffset ExpiresAt);
