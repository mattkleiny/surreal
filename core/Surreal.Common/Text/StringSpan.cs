using System.Runtime.CompilerServices;

namespace Surreal.Text;

/// <summary>Represents a span of a <see cref="string"/>.</summary>
public readonly record struct StringSpan(string? Source, int Offset, int Length)
{
  public StringSpan(string source)
    : this(source, 0, source.Length)
  {
  }

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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Match(char token) => Peek() == token;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public char Peek() => Length > 1 ? this[1] : '\0';

  public ReadOnlySpan<char> ToSpan() => Source != null ? Source.AsSpan(Offset, Length) : default;

  public override string ToString() => Source?.AsSpan(Offset, Length).ToString() ?? string.Empty;

  public static implicit operator StringSpan(string value) => new(value);
  public static implicit operator StringSpan(ReadOnlySpan<char> value) => new(value.ToString());
  public static implicit operator ReadOnlySpan<char>(StringSpan span) => span.ToSpan();
}

/// <summary>General purpose extensions for <see cref="StringSpan"/>s.</summary>
public static class StringSpanExtensions
{
  public static StringSpan AsStringSpan(this string source)
  {
    return new StringSpan(source, 0, source.Length);
  }

  public static StringSpan AsStringSpan(this string source, int offset)
  {
    return new StringSpan(source, offset, source.Length - offset);
  }

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
}
