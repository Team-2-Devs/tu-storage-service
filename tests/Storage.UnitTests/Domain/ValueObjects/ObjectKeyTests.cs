using FluentAssertions;
using Storage.Domain.ValueObjects;

namespace Storage.UnitTests.Domain.ValueObjects;

public sealed class ObjectKeyTests
{
  // Valid keys
  [Theory]
  [InlineData("images/2025/11/09/sample.jpg")]
  [InlineData(" images/2025/11/09/sample.jpg ")]
  public void TryParse_ValidObjectKey_Succeeds(string input)
  {
    var ok = ObjectKey.TryParse(input, out var key, out var err);

    ok.Should().BeTrue();
    key!.Value.Should().Be(input.Trim());
    err.Should().BeNull();
  }

  // Required validation
  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData(null)]
  public void TryParse_Required_Fails(string? input)
  {
    var ok = ObjectKey.TryParse(input, out var key, out var err);

    ok.Should().BeFalse();
    key.Should().BeNull();
    err.Should().Be("Required");
  }

  // Max length
  [Fact]
  public void TryParse_MaxLengthExceeded_Fails()
  {
    var tooLong = new string('a', 201);

    var ok = ObjectKey.TryParse(tooLong, out var key, out var err);

    ok.Should().BeFalse();
    key.Should().BeNull();
    err.Should().Be("MaxLengthExceeded");
  }

  // Leading or trailing slash
  [Theory]
  [InlineData("/leading.jpg")]
  [InlineData("trailing/")]
  public void TryParse_InvalidPathShape_Fails(string input)
  {
    var ok = ObjectKey.TryParse(input, out var key, out var err);

    ok.Should().BeFalse();
    key.Should().BeNull();
    err.Should().Be("InvalidPathShape");
  }

  // Traversal and double-slash
  [Theory]
  [InlineData("images//sample.jpg")]
  [InlineData("images/../secret.jpg")]
  public void TryParse_InvalidPathTraversal_Fails(string input)
  {
    var ok = ObjectKey.TryParse(input, out var key, out var err);

    ok.Should().BeFalse();
    key.Should().BeNull();
    err.Should().Be("InvalidPathTraversal");
  }

  // Disallowed characters
  [Theory]
  [InlineData("images/has space.jpg")]
  [InlineData("images/ä.jpg")]
  public void TryParse_InvalidCharacterSet_Fails(string input)
  {
    var ok = ObjectKey.TryParse(input, out var key, out var err);

    ok.Should().BeFalse();
    key.Should().BeNull();
    err.Should().Be("InvalidCharacterSet");
  }


  // Note:
  // Parse() and equality operators are part of the value object pattern
  // but not currently used in production code, so they are not yet tested here.
}
