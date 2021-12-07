namespace Surreal.Text;

/// <summary>Represents a span of a <see cref="string"/>.</summary>
public readonly record struct StringSpan(string? Source, int Offset, int Length)
{
  public StringSpan(string source)
    : this(source, 0, source.Length)
  {
  }

  public ReadOnlySpan<char> ToSpan()
  {
    if (Source != null)
    {
      return Source.AsSpan(Offset, Length);
    }

    return default;
  }

  public override string? ToString() => Source?.AsSpan(Offset, Length).ToString();

  public static implicit operator StringSpan(string value)            => new(value);
  public static implicit operator ReadOnlySpan<char>(StringSpan span) => span.ToSpan();
}

/// <summary>General purpose extensions for <see cref="StringSpan"/>s.</summary>
public static class StringSpanExtensions
{
  public static StringSpan AsStringSpan(this string source, int offset)
  {
    return new(source, offset, source.Length - offset);
  }

  public static StringSpan AsStringSpan(this string source, int offset, int length)
  {
    return new(source, offset, length);
  }
}