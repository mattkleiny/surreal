using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics.Grids;
using Surreal.Memory;

namespace Surreal.Graphics.Textures {
  [DebuggerDisplay("Pixmap {Width}x{Height} ~{Size}")]
  public sealed class Pixmap : IGrid<Color>, ITextureData, IDisposable {
    private readonly IDisposableBuffer<Color> buffer;

    public static async Task<Pixmap> LoadAsync(Path path) {
      await using var stream = await path.OpenInputStreamAsync();
      using var       image  = Image.Load(stream);

      var result = new Pixmap(image.Width, image.Height);

      image.GetPixelSpan().Cast<Rgba32, Color>().CopyTo(result.Span);

      return result;
    }

    public Pixmap(int width, int height) {
      Check.That(width  > 0, "width > 0");
      Check.That(height > 0, "height > 0");

      Width  = width;
      Height = height;

      buffer = Buffers.AllocateOffHeap<Color>(width * height);
    }

    public Size Size   => buffer.Size;
    public int  Width  { get; }
    public int  Height { get; }

    public TextureFormat Format => TextureFormat.RGBA8888;
    public Span<Color>   Span   => buffer.Span;

    public Color this[int x, int y] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => Span[x + y * Width];
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => Span[x + y * Width] = value;
    }

    public PixmapRegion ToRegion() => new PixmapRegion(this);

    public async Task SaveAsync(Path path) {
      await using var stream = await path.OpenOutputStreamAsync();
      using var       image  = new Image<Rgba32>(Width, Height);

      Span.Cast<Color, Rgba32>().CopyTo(image.GetPixelSpan());

      image.SaveAsPng(stream);
    }

    public void Dispose() => buffer.Dispose();

    public sealed class Loader : AssetLoader<Pixmap> {
      public override async Task<Pixmap> LoadAsync(Path path, IAssetLoaderContext context) {
        return await Pixmap.LoadAsync(path);
      }
    }
  }
}