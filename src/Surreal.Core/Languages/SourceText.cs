using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Languages {
  public abstract class SourceText {
    public static SourceText FromString(string source) => new StringText(source);

    public char this[Index index] => Span[index];
    public ReadOnlySpan<char> this[Range range] => Span[range];

    public abstract ReadOnlySpan<char> Span { get; }

    public override string ToString()            => ToString(Range.All);
    public          string ToString(Range range) => Span[range].ToString();

    [DebuggerDisplay("String source {Span.Length} characters")]
    private sealed class StringText : SourceText {
      private readonly string source;

      public StringText(string source) {
        this.source = source;
      }

      public override ReadOnlySpan<char> Span => source;
    }

    public sealed class Loader : AssetLoader<SourceText> {
      public override async Task<SourceText> LoadAsync(Path path, IAssetLoaderContext context) {
        return FromString(await path.ReadAllTextAsync());
      }
    }
  }
}