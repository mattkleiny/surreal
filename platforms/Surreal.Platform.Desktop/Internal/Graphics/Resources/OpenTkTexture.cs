using System.Runtime.CompilerServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Internal.Graphics.Resources;

[DebuggerDisplay("Texture {Width}x{Height} @ {Format} ~{Size}")]
internal sealed class OpenTkTexture : Texture
{
  public TextureHandle Id { get; } = GL.GenTexture();

  public OpenTkTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : this(data.Format, filterMode, wrapMode)
  {
    Upload(data);
  }

  public OpenTkTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : base(format, filterMode, wrapMode)
  {
    GL.BindTexture(TextureTarget.Texture2d, Id);

    GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);

    var (minFilter, magFilter) = ConvertFilterMode(filterMode);

    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int) minFilter);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int) magFilter);

    var wrapping = ConvertWrapMode(wrapMode);

    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int) wrapping);
    GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int) wrapping);
  }

  protected override unsafe void Upload(ITextureData? existingData, ITextureData newData)
  {
    GL.BindTexture(TextureTarget.Texture2d, Id);

    var (pixelFormat, pixelType) = ConvertTextureFormat(newData.Format);

    fixed (Color* pixels = newData.Pixels)
    {
      if (existingData == null || existingData.Format != newData.Format)
      {
        GL.TexImage2D(
          target: TextureTarget.Texture2d,
          level: 0,
          internalformat: 0, // TODO: fix me
          width: newData.Width,
          height: newData.Height,
          border: 0,
          format: pixelFormat,
          type: pixelType,
          pixels: new IntPtr(pixels)
        );
      }
      else
      {
        GL.TexSubImage2D(
          target: TextureTarget.Texture2d,
          level: 0,
          xoffset: 0,
          yoffset: 0,
          width: newData.Width,
          height: newData.Height,
          format: pixelFormat,
          type: pixelType,
          pixels: new IntPtr(pixels)
        );
      }
    }
  }

  public override Image Download()
  {
    var width  = 0;
    var height = 0;

    GL.BindTexture(TextureTarget.Texture2d, Id);
    GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureWidth, ref width);
    GL.GetTexParameteri(TextureTarget.Texture2d, GetTextureParameter.TextureHeight, ref height);

    var image = new Image(width, height);

    GL.GetTexImage(
      target: TextureTarget.Texture2d,
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
