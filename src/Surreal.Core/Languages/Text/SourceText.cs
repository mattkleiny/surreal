using System;

namespace Surreal.Languages.Text
{
  public abstract class SourceText
  {
    public static SourceText FromString(string source) => new StringText(source);

    public abstract int                Length { get; }
    public abstract ReadOnlySpan<char> Span   { get; }

    public char this[Index index] => Span[index];
    public ReadOnlySpan<char> this[Range range] => Span[range];

    public override string ToString()            => ToString(Range.All);
    public          string ToString(Range range) => this[range].ToString();
  }
}
