using System.Runtime.CompilerServices;

namespace Surreal.Text;

/// <summary>Represents a span of a <see cref="string" />.</summary>
public readonly record struct StringSpan(string? Source, int Offset, int Length)
{
  public StringSpan(string source)
    : this(source, 0, source.Length)
  {
  }

  /// <summary>Accesses a single character in the span.</summary>
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

  /// <summary>Accesses a range of characters in the span.</summary>
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

  /// <summary>Splits a string at the first instance of the given character, yielding the left and right halves.</summary>
  public static (StringSpan Left, StringSpan Right) Split(string input, string separator)
  {
    if (!TrySplit(input, separator, out var result))
    {
      throw new ArgumentException("The given string was not able to be split", nameof(input));
    }

    return result;
  }

  /// <summary>Splits a string at the first instance of the given character, yielding the left and right halves.</summary>
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

  /// <summary>Returns the next character from the start of the span.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public char Peek()
  {
    return Length > 1 ? this[1] : '\0';
  }

  /// <summary>Determines if the next character matches the given token.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Match(char token)
  {
    return Peek() == token;
  }

  public override int GetHashCode()
  {
    // hash code is based on the individual characters of the span.</summary>
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

  public static implicit operator StringSpan(string value)
  {
    return new StringSpan(value);
  }

  public static implicit operator StringSpan(ReadOnlySpan<char> value)
  {
    return new StringSpan(value.ToString());
  }

  public static implicit operator ReadOnlySpan<char>(StringSpan span)
  {
    return span.ToSpan();
  }
}

/// <summary>General purpose extensions for <see cref="StringSpan" />s.</summary>
public static class StringSpanExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source)
  {
    return new StringSpan(source, 0, source.Length);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source, int offset)
  {
    return new StringSpan(source, offset, source.Length - offset);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static StringSpan AsStringSpan(this string source, int offset, int length)
  {
    return new StringSpan(source, offset, length);
  }

  /// <summary>Consumes all of the next contiguous digits in the span.</summary>
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

  /// <summary>Consumes all of the next contiguous digits, including decimal places in the span.</summary>
  public static StringSpan ConsumeNumericWithFractions(this StringSpan span)
  {
    var offset = 1;
    var hasFraction = false;

    for (var i = 1; i < span.Length; i++)
    {
      if (span[i] == '.')
      {
        if (hasFraction)
        {
          break;
        }

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

  /// <summary>Consumes all alpha-numeric characters in the span</summary>
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

  /// <summary>Consumes all alpha-numeric characters in the span</summary>
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

  /// <summary>Consumes all of the given characters from the span.</summary>
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

  /// <summary>Consumes all characters until the given token is detected.</summary>
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

  public static string Highlight(this StringSpan span, string leftTerminal, string rightTerminal)
  {
    if (span.Source == null)
    {
      return string.Empty;
    }

    var builder = new StringBuilder();

    builder.Append(span.Source[..span.Offset]);
    builder.Append(leftTerminal);
    builder.Append(span.ToString());
    builder.Append(rightTerminal);
    builder.Append(span.Source[(span.Offset + span.Length)..]);

    return builder.ToString();
  }
}



