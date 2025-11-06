namespace Storage.Api.Contracts;

public sealed record PresignGetResponse(string Url, DateTimeOffset ExpiresAt);
