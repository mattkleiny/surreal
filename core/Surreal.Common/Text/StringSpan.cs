﻿namespace Surreal.Text;

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
  public char this[int index]
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get
    {
      if (Source != null)
      {
        return Source[Offset + index];
      }

      return default;
    }
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

  /// <summary>
  /// Tries to find the index of the given character in the span.
  /// </summary>
  public int IndexOf(char character)
  {
    if (Source == null)
    {
      return -1;
    }

    return Source.AsSpan(Offset, Length).IndexOf(character);
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

  /// <summary>
  /// Consumes all the next contiguous digits in the span.
  /// </summary>
  public static StringSpan ConsumeNumeric(this StringSpan span)
  {
    var offset = 1;

    for (var i = 1; i < span.Length; i++)
    {
      if (!char.IsDigit(span[i]))
      {
        break;
      }

      offset++;
    }

    return span[..offset];
  }

  /// <summary>
  /// Consumes all the next contiguous digits, including decimal places in the span.
  /// </summary>
  public static StringSpan ConsumeNumericWithFractions(this StringSpan span)
  {
    var offset = 1;
    var hasFraction = false;

    for (var i = 1; i < span.Length; i++)
    {
      if (span[i] == '.')
      {
        if (hasFraction) break;
        hasFraction = true;
      }
      else if (!char.IsDigit(span[i]))
      {
        break;
      }

      offset++;
    }

    return span[..offset];
  }

  /// <summary>
  /// Consumes all alpha-numeric characters in the span.
  /// </summary>
  public static StringSpan ConsumeAlphaNumeric(this StringSpan span)
  {
    var offset = 1;

    for (var i = 1; i < span.Length; i++)
    {
      var character = span[i];

      if (!char.IsLetterOrDigit(character) && character != '_')
      {
        break;
      }

      offset++;
    }

    return span[..offset];
  }

  /// <summary>
  /// Consumes all alpha-numeric characters in the span.
  /// </summary>
  public static StringSpan ConsumeAny(this StringSpan span, HashSet<char> characters)
  {
    var offset = 1;

    for (var i = 1; i < span.Length; i++)
    {
      if (!characters.Contains(span[i]))
      {
        break;
      }

      offset++;
    }

    return span[..offset];
  }

  /// <summary>
  /// Consumes all the given characters from the span.
  /// </summary>
  public static StringSpan ConsumeWhile(this StringSpan span, char token)
  {
    var offset = 1;

    for (var i = 1; i < span.Length; i++)
    {
      if (span[i] != token)
      {
        break;
      }

      offset++;
    }

    return span[..offset];
  }

  /// <summary>
  /// Consumes all characters until the given token is detected.
  /// </summary>
  public static StringSpan ConsumeUntil(this StringSpan span, char token)
  {
    var offset = 1;

    for (var i = 1; i < span.Length; i++)
    {
      offset++;

      if (span[i] == token)
      {
        break;
      }
    }

    return span[..offset];
  }
}
