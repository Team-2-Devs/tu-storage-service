using FluentAssertions;
using Storage.Domain.ValueObjects;

namespace Storage.UnitTests.Domain.ValueObjects;

public sealed class ContentTypeTests
{
  // Allowed types
  [Theory]
  [InlineData("image/jpeg")]
  [InlineData("image/png")]
  [InlineData("image/webp")]
  public void TryParse_AllowedType_Succeeds(string input)
  {
    var ok = ContentType.TryParse(input, out var ct, out var err);

    ok.Should().BeTrue();
    ct!.Value.Should().Be(input);
    err.Should().BeNull();
  }

  // Normalization (trim + case)
  [Theory]
  [InlineData("IMAGE/JPEG", "image/jpeg")]
  [InlineData(" image/png  ", "image/png")]
  [InlineData(" IMAGE/WEBP ", "image/webp")]
  public void TryParse_ValidInput_Normalizes(string input, string expected)
  {
    var ok = ContentType.TryParse(input, out var ct, out var err);

    ok.Should().BeTrue();
    ct!.Value.Should().Be(expected);
    err.Should().BeNull();
  }

  // Required validation
  [Theory]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData(null)]
  public void TryParse_NullOrEmpty_Fails(string? input)
  {
    var ok = ContentType.TryParse(input, out var ct, out var err);

    ok.Should().BeFalse();
    ct.Should().BeNull();
    err.Should().Be("Required");
  }

  // Unsupported types
  [Theory]
  [InlineData("text/plain")]
  [InlineData("image/gif")]
  public void TryParse_UnsupportedType_Fails(string input)
  {
    var ok = ContentType.TryParse(input, out var ct, out var err);

    ok.Should().BeFalse();
    ct.Should().BeNull();
    err.Should().Be("UnsupportedContentType");
  }


  // Note:
  // Parse() and equality operators are part of the value object pattern
  // but not currently used in production code, so they are not yet tested here.
}
