namespace Storage.Domain.ValueObjects;

/// <summary>
/// Represents a validated time-to-live (TTL) duration in seconds for presigned URLs.
/// </summary>
public readonly struct TtlSeconds : IEquatable<TtlSeconds>
{
  /// <summary>
  /// The validated TTL value in seconds.
  /// </summary>
  public int Value { get; }

  private TtlSeconds(int value) => Value = value;

  /// <summary>
  /// Attempts to parse and validate a <see cref="TtlSeconds"/> from the specified input.
  /// </summary>
  /// <param name="input">Raw TTL value in seconds.</param>
  /// <param name="ttl">The resulting <see cref="TtlSeconds"/> if parsing succeeds; otherwise, <c>default</c>.</param>
  /// <param name="error">An error code explaining why validation failed.</param>
  /// <returns><c>true</c> if valid and within range; otherwise, <c>false</c>.</returns>
  public static bool TryParse(int input, out TtlSeconds ttl, out string? error)
  {
    ttl = default;
    error = null;

    if (input < 1 || input > 60)
    {
      error = "Range1To60";
      return false;
    }

    ttl = new TtlSeconds(input);
    return true;
  }

  public static TtlSeconds Parse(int input)
  {
    if (!TryParse(input, out var ttl, out var error))
      throw new ArgumentException($"Invalid TtlSeconds: {error}", nameof(input));

    return ttl;
  }

  public override string ToString() => Value.ToString();

  /// <summary>
  /// Implicitly converts the <see cref="TtlSeconds"/> to its integer value.
  /// </summary>
  public static implicit operator int(TtlSeconds value) => value.Value;

  // Equality members
  public bool Equals(TtlSeconds other) => Value == other.Value;
  public override bool Equals(object? obj) => obj is TtlSeconds other && Equals(other);
  public override int GetHashCode() => Value;
  public static bool operator ==(TtlSeconds left, TtlSeconds right) => left.Equals(right);
  public static bool operator !=(TtlSeconds left, TtlSeconds right) => !left.Equals(right);
}
