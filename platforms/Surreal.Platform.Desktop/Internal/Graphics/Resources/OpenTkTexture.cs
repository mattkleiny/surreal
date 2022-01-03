using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Internal.Graphics.Resources;

[DebuggerDisplay("Texture {Width}x{Height} @ {Format} ~{Size}")]
internal sealed class OpenTkTexture : Texture, IHasNativeId
{
  public int Id { get; } = GL.GenTexture();

  public OpenTkTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : this(data.Format, filterMode, wrapMode)
  {
    Upload(data);
  }

  public OpenTkTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : base(format, filterMode, wrapMode)
  {
    GL.BindTexture(TextureTarget.Texture2D, Id);

    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

    var (minFilter, magFilter) = ConvertFilterMode(filterMode);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) minFilter);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) magFilter);

    var wrapping = ConvertWrapMode(wrapMode);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) wrapping);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) wrapping);
  }

  protected override unsafe void Upload(ITextureData? existingData, ITextureData newData)
  {
    GL.BindTexture(TextureTarget.Texture2D, Id);

    var (pixelFormat, pixelType) = ConvertTextureFormat(newData.Format);

    fixed (Color* pixels = newData.Pixels)
    {
      if (existingData == null || existingData.Format != newData.Format)
      {
        GL.TexImage2D(
          target: TextureTarget.Texture2D,
          level: 0,
          internalformat: PixelInternalFormat.Rgba,
          width: newData.Width,
          height: newData.Height,
          border: 0,
          format: pixelFormat,
          type: pixelType,
          pixels: ref Unsafe.AsRef<Color>(pixels)
        );
      }
      else
      {
        GL.TexSubImage2D(
          target: TextureTarget.Texture2D,
          level: 0,
          xoffset: 0,
          yoffset: 0,
          width: newData.Width,
          height: newData.Height,
          format: pixelFormat,
          type: pixelType,
          pixels: ref Unsafe.AsRef<Color>(pixels)
        );
      }
    }
  }

  public override Image Download()
  {
    GL.BindTexture(TextureTarget.Texture2D, Id);
    GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureWidth, out int width);
    GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureHeight, out int height);

    var image = new Image(width, height);

    GL.GetTexImage(
      target: TextureTarget.Texture2D,
      level: 0,
      format: PixelFormat.Rgba,
      type: PixelType.UnsignedByte,
      pixels: ref image.Pixels.GetPinnableReference()
    );

    return image;
  }

  protected override void Dispose(bool managed)
  {
    GL.DeleteTexture(Id);

    base.Dispose(managed);
  }

  private static (TextureMinFilter MinFilter, TextureMagFilter MagFilter) ConvertFilterMode(TextureFilterMode mode)
  {
    switch (mode)
    {
      case TextureFilterMode.Linear: return (TextureMinFilter.Linear, TextureMagFilter.Linear);
      case TextureFilterMode.Point:  return (TextureMinFilter.Nearest, TextureMagFilter.Nearest);

      default:
        throw new ArgumentException($"An unrecognized filter mode was provided: {mode}", nameof(mode));
    }
  }

  private static OpenTK.Graphics.OpenGL.TextureWrapMode ConvertWrapMode(TextureWrapMode mode)
  {
    switch (mode)
    {
      case TextureWrapMode.Clamp:  return OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;
      case TextureWrapMode.Repeat: return OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;

      default:
        throw new ArgumentException($"An unrecognized wrap mode was provided: {mode}", nameof(mode));
    }
  }

  private static (PixelFormat Format, PixelType Type) ConvertTextureFormat(TextureFormat format)
  {
    switch (format)
    {
      case TextureFormat.Rgba8888: return (PixelFormat.Rgba, PixelType.UnsignedByte);

      default:
        throw new ArgumentException($"An unrecognized texture format was provided: {format}", nameof(format));
    }
  }
}
