using FluentAssertions;
using Storage.Domain.ValueObjects;

namespace Storage.UnitTests.Domain.ValueObjects;

public sealed class TtlSecondsTests
{
  // Valid TTL values
  [Theory]
  [InlineData(1)]
  [InlineData(300)]
  [InlineData(3600)]
  public void TryParse_ValidTtl_Succeeds(int input)
  {
    var ok = TtlSeconds.TryParse(input, out var ttl, out var err);

    ok.Should().BeTrue();
    ttl.Value.Should().Be(input);
    err.Should().BeNull();
  }

  // Below minimum range
  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void TryParse_BelowRange_Fails(int input)
  {
    var ok = TtlSeconds.TryParse(input, out var ttl, out var err);

    ok.Should().BeFalse();
    ttl.Value.Should().Be(0);
    err.Should().Be("Range1To3600");
  }

  // Above maximum range
  [Theory]
  [InlineData(3601)]
  [InlineData(9999)]
  public void TryParse_AboveRange_Fails(int input)
  {
    var ok = TtlSeconds.TryParse(input, out var ttl, out var err);

    ok.Should().BeFalse();
    ttl.Value.Should().Be(0);
    err.Should().Be("Range1To3600");
  }


  // Note:
  // Parse() and equality operators are part of the value object pattern
  // but not currently used in production code, so they are not yet tested here.
}

