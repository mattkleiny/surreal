using System;

namespace Surreal.Languages.Text {
  internal sealed class StringText : SourceText {
    private readonly string source;

    public StringText(string source) {
      this.source = source;
    }

    public override int                Length => source.Length;
    public override ReadOnlySpan<char> Span   => source;
  }
}