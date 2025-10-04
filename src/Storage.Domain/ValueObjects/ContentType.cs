using System.Diagnostics.CodeAnalysis;

namespace Storage.Domain.ValueObjects;

/// <summary>
/// Represents a validated MIME content type for image uploads.
/// </summary>
public sealed class ContentType : IEquatable<ContentType>
{
  public string Value { get; }

  private ContentType(string value) => Value = value;

  /// <summary>
  /// Validates and parses a content type string into a <see cref="ContentType"/>.
  /// </summary>
  /// <param name="input">Raw MIME type, trimmed before validation.</param>
  /// <param name="contentType">The parsed <see cref="ContentType"/> if valid; otherwise, <c>null</c>.</param>
  /// <param name="error">Error code if validation fails.</param>
  /// <returns><c>true</c> if valid and parsed successfully; otherwise, <c>false</c>.</returns>
  public static bool TryParse(string? input, [NotNullWhen(true)] out ContentType? contentType, out string? error)
  {
    input = input?.Trim();
    contentType = null;
    error = null;

    if (string.IsNullOrEmpty(input))
    {
      error = "Required";
      return false;
    }

    if (!input.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
    {
      error = "MustStartWithImageSlash";
      return false;
    }

    contentType = new ContentType(input.ToLowerInvariant());
    return true;
  }

  /// <summary>
  /// Parses and validates a <see cref="ContentType"/> from the specified input.
  /// Throws if the input is invalid.
  /// </summary>
  /// <param name="input">Raw content type string to validate.</param>
  /// <returns>A validated <see cref="ContentType"/>.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="input"/> is <c>null</c>, empty, or not an image content type.
  /// </exception>
  public static ContentType Parse(string input)
  {
    ArgumentNullException.ThrowIfNullOrEmpty(input, nameof(input));

    if (!TryParse(input, out var contentType, out var error))
    {
      throw new ArgumentException($"Invalid contentType: {error}", nameof(input));
    }

    return contentType!;
  }

  public override string ToString() => Value;

  // Equality members
  public bool Equals(ContentType? other) => other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);
  public override bool Equals(object? obj) => Equals(obj as ContentType);
  public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
  public static bool operator ==(ContentType? left, ContentType? right) => Equals(left, right);
  public static bool operator !=(ContentType? left, ContentType? right) => !Equals(left, right);
}
