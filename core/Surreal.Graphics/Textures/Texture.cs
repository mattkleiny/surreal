﻿using Surreal.Colors;
using Surreal.Graphics.Images;
using Surreal.Maths;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Formats for a <see cref="Texture" />.
/// </summary>
public enum TextureFormat
{
  R8,
  Rg8,
  Rgb8,
  Rgba8,
  R,
  Rg,
  Rgb,
  Rgba
}

/// <summary>
/// Wrapping modes for a <see cref="Texture" />.
/// </summary>
public enum TextureWrapMode
{
  Clamp,
  Repeat
}

/// <summary>
/// Filter modes for a <see cref="Texture" />.
/// </summary>
public enum TextureFilterMode
{
  Point,
  Linear
}

/// <summary>
/// A texture that can be uploaded to the GPU.
/// </summary>
[DebuggerDisplay("Texture {Width}x{Height} (Format {Format})")]
public sealed class Texture(IGraphicsContext context,
  TextureFormat format = TextureFormat.Rgba8,
  TextureFilterMode filterMode = TextureFilterMode.Point,
  TextureWrapMode wrapMode = TextureWrapMode.Clamp) : GraphicsResource, IHasSizeEstimate
{
  private TextureFilterMode _filterMode = TextureFilterMode.Point;
  private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

  public IGraphicsContext Context { get; } = context;

  public int Width { get; private set; }
  public int Height { get; private set; }

  public GraphicsHandle Handle { get; } = context.Backend.CreateTexture(filterMode, wrapMode);
  public TextureFormat Format { get; set; } = format;

  public TextureFilterMode FilterMode
  {
    get => _filterMode;
    set
    {
      _filterMode = value;
      Context.Backend.SetTextureFilterMode(Handle, value);
    }
  }

  public TextureWrapMode WrapMode
  {
    get => _wrapMode;
    set
    {
      _wrapMode = value;
      Context.Backend.SetTextureWrapMode(Handle, value);
    }
  }

  public Size Size { get; private set; }

  /// <summary>
  /// Creates a colored 1x1 texture.
  /// </summary>
  public static Texture CreateColored(IGraphicsContext context, Color color, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(context, format);

    texture.WritePixels<Color>(1, 1, stackalloc Color[] { color });

    return texture;
  }

  /// <summary>
  /// Creates a texture from random noise.
  /// </summary>
  public static Texture CreateNoise(IGraphicsContext context, int width, int height, Seed seed = default,
    TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(context, format);
    var random = seed.ToRandom();

    var pixels = new SpanGrid<Color>(new Color[width * height], width);

    for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
    {
      var color = random.NextFloat();

      pixels[x, y] = new Color(color, color, color);
    }

    texture.WritePixels<Color>(width, height, pixels);

    return texture;
  }

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public Memory<T> ReadPixels<T>()
    where T : unmanaged
  {
    return Context.Backend.ReadTextureData<T>(Handle);
  }

  public void ReadPixels<T>(Span<T> buffer)
    where T : unmanaged
  {
    Context.Backend.ReadTextureData(Handle, buffer);
  }

  public Memory<T> ReadPixelsSub<T>(int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    return Context.Backend.ReadTextureSubData<T>(Handle, offsetX, offsetY, width, height);
  }

  public void ReadPixelsSub<T>(Span<T> buffer, int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    Context.Backend.ReadTextureSubData(Handle, buffer, offsetX, offsetY, width, height);
  }

  public void WritePixels<T>(int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Width = width;
    Height = height;
    Size = pixels.CalculateSize();

    Context.Backend.WriteTextureData(Handle, width, height, pixels, Format);
  }

  public void WritePixelsSub<T>(int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Context.Backend.WriteTextureSubData(Handle, offsetX, offsetY, width, height, pixels, Format);
  }

  public void WritePixels(Image image)
  {
    var pixels = image.Pixels.ToReadOnlySpan();

    WritePixels(image.Width, image.Height, pixels);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      Context.Backend.DeleteTexture(Handle);
    }

    base.Dispose(managed);
  }
}
