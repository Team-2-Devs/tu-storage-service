using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Storage.Domain.ValueObjects;

/// <summary>
/// Represents a validated storage object key used for addressing files in storage.
/// </summary>
public sealed partial class ObjectKey : IEquatable<ObjectKey>
{
  private const int MaxLength = 200;

  public string Value { get; }

  private ObjectKey(string value) => Value = value;

  [GeneratedRegex(@"^[A-Za-z0-9/_\-.]+$", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
  private static partial Regex Allowed();

  /// <summary>
  /// Attempts to parse and validate an <see cref="ObjectKey"/> from the specified input.
  /// </summary>
  /// <param name="input">Raw key string to validate (leading/trailing whitespace is trimmed).</param>
  /// <param name="key">The resulting <see cref="ObjectKey"/> if parsing succeeds; otherwise, null.</param>
  /// <param name="error">An error code explaining why validation failed; otherwise, null.</param>
  /// <returns>true if the input represents a valid <see cref="ObjectKey"/>; otherwise, false.</returns>
  public static bool TryParse(string? input, [NotNullWhen(true)] out ObjectKey? key, out string? error)
  {
    input = input?.Trim();
    key = null;
    error = null;

    if (string.IsNullOrEmpty(input))
    {
      error = "Required";
      return false;
    }

    if (input.Length > MaxLength)
    {
      error = "MaxLengthExceeded";
      return false;
    }

    if (input.StartsWith('/') ||
        input.EndsWith('/'))
    {
      error = "InvalidPathShape";
      return false;
    }

    if (input.Contains("..", StringComparison.Ordinal) ||
        input.Contains("//", StringComparison.Ordinal))
    {
      error = "InvalidPathTraversal";
      return false;
    }

    if (!Allowed().IsMatch(input))
    {
      error = "InvalidCharacterSet";
      return false;
    }

    key = new ObjectKey(input);
    return true;
  }

  /// <summary>
  /// Parses and validates an <see cref="ObjectKey"/> from the specified input.
  /// Throws if the input is invalid.
  /// </summary>
  /// <param name="input">Raw key string to validate.</param>
  /// <returns>A validated <see cref="ObjectKey"/>.</returns>
  /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
  /// <exception cref="ArgumentException">Thrown when <paramref name="input"/> is not a valid object key.</exception>
  public static ObjectKey Parse(string input)
  {
    ArgumentNullException.ThrowIfNull(input, nameof(input));

    if (!TryParse(input, out var key, out var error))
      throw new ArgumentException($"Invalid ObjectKey: {error}", nameof(input));

    return key!;
  }

  public override string ToString() => Value;

  // Equality members
  public bool Equals(ObjectKey? other) => other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);
  public override bool Equals(object? obj) => Equals(obj as ObjectKey);
  public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
  public static bool operator ==(ObjectKey? left, ObjectKey? right) => Equals(left, right);
  public static bool operator !=(ObjectKey? left, ObjectKey? right) => !Equals(left, right);

}
