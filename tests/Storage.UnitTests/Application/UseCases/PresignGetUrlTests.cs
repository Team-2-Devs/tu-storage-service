using FluentAssertions;
using Storage.Application.UseCases;
using Storage.Application.Ports.Inbound.Contracts;
using Storage.UnitTests.Common.Builders;
using Storage.UnitTests.Common.Doubles;

namespace Storage.UnitTests.Application.UseCases;

public sealed class PresignGetUrlTests
{
  // Happy path
  [Fact]
  public async Task HandleAsync_ValidCommand_SucceedsAndCallsPresigner()
  {
    var presigner = new FakeObjectStoragePresigner
    {
      UrlToReturn = "http://local/get",
      ExpiresAtToReturn = DateTimeOffset.Parse("2030-01-01T00:00:00Z")
    };
    var sut = new PresignGetUrl(presigner);

    var cmd = new PresignCommandBuilder()
      .WithKey("images/2025/11/09/sample.jpg")
      .WithTtl(300)
      .BuildGet();

    var result = await sut.HandleAsync(cmd);

    var success = result as PresignGetUrlResult.Success;
    success.Should().NotBeNull();
    success!.Url.Should().Be("http://local/get");
    success.ExpiresAt.Should().Be(DateTimeOffset.Parse("2030-01-01T00:00:00Z"));

    presigner.LastMethod.Should().Be("GET");
    presigner.LastKey!.Value.Should().Be("images/2025/11/09/sample.jpg");
    presigner.LastTtl!.Value.Value.Should().Be(300); // Double .Value: unwrap Nullable<TtlSeconds>, then get int
    presigner.LastContentType.Should().BeNull(); // GET doesn't use ContentType
  }

  // Invalid key
  [Fact]
  public async Task HandleAsync_InvalidKey_ReturnsInvalid_AndSkipsPresigner()
  {
    var presigner = new FakeObjectStoragePresigner();
    var sut = new PresignGetUrl(presigner);

    var cmd = new PresignCommandBuilder()
      .WithKey("images//bad.jpg")
      .BuildGet();

    var result = await sut.HandleAsync(cmd);

    var invalid = result as PresignGetUrlResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("key");

    presigner.LastMethod.Should().BeNull();
  }

  // Invalid ttl
  [Fact]
  public async Task HandleAsync_InvalidTtl_ReturnsInvalid_AndSkipsPresigner()
  {
    var presigner = new FakeObjectStoragePresigner();
    var sut = new PresignGetUrl(presigner);

    var cmd = new PresignCommandBuilder()
      .WithTtl(0)
      .BuildGet();

    var result = await sut.HandleAsync(cmd);

    var invalid = result as PresignGetUrlResult.Invalid;
    invalid.Should().NotBeNull();
    invalid!.Errors.Should().ContainKey("ttlSec");

    presigner.LastMethod.Should().BeNull();
  }


  // Note:
  // Cancellation and exceptional paths are not covered for the semester scope.
}
