namespace Surreal.Text;

/// <summary>
/// Represents a span of a <see cref="string" />.
/// </summary>
public readonly record struct StringSpan(string? Source, int Offset, int Length)
{
  /// <summary>
  /// Splits a string at the first instance of the given character, yielding the left and right halves.
  /// </summary>
  public static (StringSpan Left, StringSpan Right) Split(string input, string separator)
  {
    if (!TrySplit(input, separator, out var result))
    {
      throw new ArgumentException("The given string was not able to be split", nameof(input));
    }

    return result;
  }

  /// <summary>
  /// Splits a string at the first instance of the given character, yielding the left and right halves.
  /// </summary>
  public static bool TrySplit(string input, string separator, out (StringSpan Left, StringSpan Right) result)
  {
    var index = input.IndexOf(separator, StringComparison.Ordinal);
    if (index > -1)
    {
      var left = input.AsStringSpan(0, index);
      var right = input.AsStringSpan(index + separator.Length);

      result = (left, right);
      return true;
    }

    result = default;
    return false;
  }

  public StringSpan(string source)
    : this(source, 0, source.Length)
  {
  }

  /// <summary>
  /// Accesses a single character in the span.
  /// </summary>
  public char this[Index index]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get
    {
      if (Source != null)
      {
        var offset = index.GetOffset(Length);

        return Source[Offset + offset];
      }

      return default;
    }
  }

  /// <summary>
  /// Accesses a range of characters in the span.
  /// </summary>
  public StringSpan this[Range range]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get
    {
      if (Source != null)
      {
        var (offset, length) = range.GetOffsetAndLength(Length);

        return new StringSpan(Source, Offset + offset, length);
      }

      return default;
    }
  }

  public bool Equals(StringSpan other)
  {
    // equality is based on the individual characters of the span.
    return ToSpan().SequenceEqual(other.ToSpan());
  }

  public override int GetHashCode()
  {
    return string.GetHashCode(ToSpan());
  }

  public ReadOnlySpan<char> ToSpan()
  {
    return Source != null ? Source.AsSpan(Offset, Length) : default;
  }

  public override string ToString()
  {
    return Source?.AsSpan(Offset, Length).ToString() ?? string.Empty;
  }

  public static implicit operator StringSpan(string value) => new(value);
  public static implicit operator StringSpan(ReadOnlySpan<char> value) => new(value.ToString());
  public static implicit operator ReadOnlySpan<char>(StringSpan span) => span.ToSpan();
}

/// <summary>
/// General purpose extensions for <see cref="StringSpan" />s.
/// </summary>
public static class StringSpanExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source)
    => new(source, 0, source.Length);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source, int offset)
    => new(source, offset, source.Length - offset);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source, int offset, int length)
    => new(source, offset, length);
}
