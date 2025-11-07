namespace Storage.Api.Contracts;

/// <summary>Response containing a presigned GET URL and its expiry time.</summary>
public sealed record PresignGetResponse(string Url, DateTimeOffset ExpiresAt);
