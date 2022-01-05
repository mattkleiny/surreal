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
    get
    {
      if (Source == null) return default;
      var offset = index.GetOffset(Length);

      return Source[Offset + offset];
    }
  }

  public StringSpan this[Range range]
  {
    get
    {
      if (Source == null) return default;
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new StringSpan(Source, Offset + offset, length);
    }
  }

  public ReadOnlySpan<char> ToSpan()
  {
    if (Source == null) return default;

    return Source.AsSpan(Offset, Length);
  }

  public override string? ToString()
  {
    return Source?.AsSpan(Offset, Length).ToString();
  }

  public static implicit operator StringSpan(string value)            => new(value);
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
}
