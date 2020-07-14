using System;

namespace Surreal.Languages {
  public abstract class SourceText {
    public static SourceText FromString(string source) => new StringText(source);

    public abstract ReadOnlySpan<char> Span { get; }

    public override string ToString()            => ToString(Range.All);
    public          string ToString(Range range) => Span[range].ToString();

    private sealed class StringText : SourceText {
      private readonly string source;

      public StringText(string source) {
        this.source = source;
      }

      public override ReadOnlySpan<char> Span => source;
    }
  }
}