using System;

namespace Surreal.Text
{
  /// <summary>Represents a span of a <see cref="string"/>.</summary>
  public readonly struct StringSpan : IEquatable<StringSpan>
  {
    public string? Source { get; }
    public int     Offset { get; }
    public int     Length { get; }

    public StringSpan(string source)
        : this(source, 0, source.Length)
    {
    }

    public StringSpan(string source, int offset, int length)
    {
      Source = source;
      Offset = offset;
      Length = length;
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

    public          bool Equals(StringSpan other) => Source == other.Source && Offset == other.Offset && Length == other.Length;
    public override bool Equals(object? obj)      => obj is StringSpan other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Source, Offset, Length);

    public static bool operator ==(StringSpan left, StringSpan right) => left.Equals(right);
    public static bool operator !=(StringSpan left, StringSpan right) => !left.Equals(right);

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
}
