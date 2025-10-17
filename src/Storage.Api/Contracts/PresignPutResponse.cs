namespace Storage.Api.Contracts;

public sealed record PresignPutResponse(string Url, DateTimeOffset ExpiresAt);
