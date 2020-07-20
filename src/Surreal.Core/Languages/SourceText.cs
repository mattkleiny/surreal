using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Languages {
  public abstract class SourceText : IDisposable {
    public static       SourceText       FromString(string source) => new StringText(source);
    public static async Task<SourceText> FromPathAsync(Path path)  => new MemoryMappedText(path, await path.GetSizeAsync());

    public char this[Index index] => Span[index];
    public ReadOnlySpan<char> this[Range range] => Span[range];

    public abstract ReadOnlySpan<char> Span { get; }

    public override string ToString()            => ToString(Range.All);
    public          string ToString(Range range) => Span[range].ToString();

    public virtual void Dispose() {
    }

    [DebuggerDisplay("String source {Span.Length} characters")]
    private sealed class StringText : SourceText {
      private readonly string source;

      public StringText(string source) {
        this.source = source;
      }

      public override ReadOnlySpan<char> Span => source;
    }

    [DebuggerDisplay("Memory-mapped source {Span.Length} characters")]
    private sealed class MemoryMappedText : SourceText {
      private readonly IDisposableBuffer<char> buffer;

      public MemoryMappedText(Path path, int size) {
        // TODO: interpret this correctly, somehow
        buffer = Buffers.MapFromFile<char>(path.Target, 0, size);
      }

      public override ReadOnlySpan<char> Span => buffer.Span;

      public override void Dispose() => buffer.Dispose();
    }

    public sealed class Loader : AssetLoader<SourceText> {
      public override async Task<SourceText> LoadAsync(Path path, IAssetLoaderContext context) {
        return FromString(await path.ReadAllTextAsync());
      }
    }
  }
}