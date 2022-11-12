﻿using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Images;

/// <summary>An image of colored pixels.</summary>
[DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
public sealed class Image : IDisposable
{
  private Image<Rgba32> _image;

  public Image(int width, int height)
  {
    Debug.Assert(width > 0, "width > 0");
    Debug.Assert(height > 0, "height > 0");

    _image = new Image<Rgba32>(width, height);
  }

  public Image(ReadOnlySpanGrid<Color32> pixels)
    : this(pixels.Width, pixels.Height)
  {
    pixels.ToReadOnlySpan().CopyTo(Pixels);
  }

  private Image(Image<Rgba32> image)
  {
    _image = image;
  }

  public int Width => _image.Width;
  public int Height => _image.Height;
  public Size Size => Pixels.ToSpan().CalculateSize();

  public SpanGrid<Color32> Pixels
  {
    get
    {
      if (!_image.TryGetSinglePixelSpan(out var span))
      {
        throw new InvalidOperationException("The image span is not contiguous, unable to access pixels!");
      }

      var pixels = MemoryMarshal.Cast<Rgba32, Color32>(span);

      return new SpanGrid<Color32>(pixels, Width);
    }
  }

  public void Dispose()
  {
    _image.Dispose();
  }

  public static Image Load(VirtualPath path)
  {
    using var stream = path.OpenInputStream();

    // load the image
    var image = SixLabors.ImageSharp.Image.Load(stream);
    if (image is Image<Rgba32> rgba)
      // we're already in the right format
    {
      return new Image(rgba);
    }

    // we need to convert to RGBA
    using (image)
    {
      return new Image(image.CloneAs<Rgba32>());
    }
  }

  public static async ValueTask<Image> LoadAsync(VirtualPath path)
  {
    await using var stream = await path.OpenInputStreamAsync();

    // load the image
    var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);
    if (image is Image<Rgba32> rgba)
      // we're already in the right format
    {
      return new Image(rgba);
    }

    // we need to convert to RGBA
    using (image)
    {
      return new Image(image.CloneAs<Rgba32>());
    }
  }

  public void Save(VirtualPath path)
  {
    using var stream = path.OpenOutputStream();

    _image.SaveAsPng(stream);
  }

  public async ValueTask SaveAsync(VirtualPath path)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await _image.SaveAsPngAsync(stream);
  }

  /// <summary>Swaps the underlying image content, for hot-reloading.</summary>
  internal void ReplaceImage(Image other)
  {
    _image = other._image;
  }
}

/// <summary>The <see cref="AssetLoader{T}" /> for <see cref="Image" />s.</summary>
public sealed class ImageLoader : AssetLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = ImmutableHashSet.Create(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga");

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && Extensions.Contains(context.Path.Extension);
  }

  public override async Task<Image> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var image = await Image.LoadAsync(context.Path);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Image>(ReloadAsync);
    }

    return image;
  }

  private static async Task<Image> ReloadAsync(AssetLoaderContext context, Image image, CancellationToken cancellationToken = default)
  {
    image.ReplaceImage(await Image.LoadAsync(context.Path));

    return image;
  }
}



