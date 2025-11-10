using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Storage.IntegrationTests.Common;

namespace Storage.IntegrationTests.Api;

public sealed class PresignEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
  private readonly HttpClient _client;

  public PresignEndpointsTests(CustomWebApplicationFactory factory)
  {
    _client = factory.CreateClient();
  }

  private sealed record PresignGetRequest(string Key, int TtlSec);
  private sealed record PresignPutRequest(string Key, string ContentType, int TtlSec);
  private sealed record PresignResponse(string Url, DateTimeOffset ExpiresAt);

  // Happy path
  [Fact]
  public async Task PresignGet_ValidRequest_Returns200_WithUrlAndExpiry()
  {
    var request = new PresignGetRequest("images/2025/11/09/sample.jpg", 300);

    var response = await _client.PostAsJsonAsync("/internal/v1/storage/presign-get", request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var body = await response.Content.ReadFromJsonAsync<PresignResponse>();
    body.Should().NotBeNull();
    body!.Url.Should().Be("http://local/presigned");
    body.ExpiresAt.Should().Be(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
  }

  [Fact]
  public async Task PresignPut_ValidRequest_Returns200_WithUrlAndExpiry()
  {
    var request = new PresignPutRequest("images/2025/11/09/sample.jpg", "image/jpeg", 300);

    var response = await _client.PostAsJsonAsync("/internal/v1/storage/presign-put", request);

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var body = await response.Content.ReadFromJsonAsync<PresignResponse>();
    body.Should().NotBeNull();
    body!.Url.Should().Be("http://local/presigned");
    body.ExpiresAt.Should().Be(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
  }

  // Validation
  [Fact]
  public async Task PresignGet_InvalidKey_Returns422()
  {
    var request = new PresignGetRequest("images//bad.jpg", 300);

    var response = await _client.PostAsJsonAsync("/internal/v1/storage/presign-get", request);

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }


  [Fact]
  public async Task PresignPut_InvalidContentType_Returns422()
  {
    var request = new PresignPutRequest("images/2025/11/09/sample.jpg", "text/plain", 300);

    var response = await _client.PostAsJsonAsync("/internal/v1/storage/presign-put", request);

    response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
  }


  // Note:
  // These tests verify end-to-end API behavior through the HTTP pipeline.
  // External dependencies are replaced with deterministic test doubles.
}
