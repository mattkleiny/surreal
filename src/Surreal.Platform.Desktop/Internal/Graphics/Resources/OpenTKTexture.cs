using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Textures;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Platform.Internal.Graphics.Resources {
  [DebuggerDisplay("Texture {Width}x{Height} @ {Format} ~{Size}")]
  internal sealed class OpenTKTexture : Texture {
    public readonly int Id = GL.GenTexture();

    public OpenTKTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode)
        : this(data.Format, filterMode, wrapMode) {
      Upload(data);
    }

    public OpenTKTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
        : base(format, filterMode, wrapMode) {
      GL.BindTexture(TextureTarget.Texture2D, Id);

      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      var (minFilter, magFilter) = ConvertFilterMode(filterMode);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) minFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) magFilter);

      var wrapping = ConvertWrapMode(wrapMode);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) wrapping);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) wrapping);
    }

    protected override void Upload(ITextureData? existingData, ITextureData newData) {
      GL.BindTexture(TextureTarget.Texture2D, Id);

      var (pixelFormat, pixelType) = ConvertTextureFormat(newData.Format);

      if (existingData == null || existingData.Format != newData.Format) {
        GL.TexImage2D(
            target: TextureTarget.Texture2D,
            level: 0,
            internalformat: PixelInternalFormat.Rgba,
            width: newData.Width,
            height: newData.Height,
            border: 0,
            format: pixelFormat,
            type: pixelType,
            pixels: ref newData.Span.GetPinnableReference()
        );
      }
      else {
        GL.TexSubImage2D(
            target: TextureTarget.Texture2D,
            level: 0,
            xoffset: 0,
            yoffset: 0,
            width: newData.Width,
            height: newData.Height,
            format: pixelFormat,
            type: pixelType,
            pixels: ref newData.Span.GetPinnableReference()
        );
      }
    }

    public override void Download(Pixmap pixmap) {
      var pixels = pixmap.Span;

      GL.BindTexture(TextureTarget.Texture2D, Id);
      GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixels.GetPinnableReference());
    }

    protected override void Dispose(bool managed) {
      GL.DeleteTexture(Id);

      base.Dispose(managed);
    }

    private static (TextureMinFilter, TextureMagFilter) ConvertFilterMode(TextureFilterMode mode) {
      switch (mode) {
        case TextureFilterMode.Linear: return (TextureMinFilter.Linear, TextureMagFilter.Linear);
        case TextureFilterMode.Point:  return (TextureMinFilter.Nearest, TextureMagFilter.Nearest);

        default:
          throw new ArgumentException($"An unrecognized filter mode was provided: {mode}", nameof(mode));
      }
    }

    private static OpenTK.Graphics.OpenGL.TextureWrapMode ConvertWrapMode(TextureWrapMode mode) {
      switch (mode) {
        case TextureWrapMode.Clamp:  return OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;
        case TextureWrapMode.Repeat: return OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;

        default:
          throw new ArgumentException($"An unrecognized wrap mode was provided: {mode}", nameof(mode));
      }
    }

    private static (PixelFormat, PixelType) ConvertTextureFormat(TextureFormat format) {
      switch (format) {
        case TextureFormat.RGBA8888: return (PixelFormat.Rgba, PixelType.UnsignedByte);

        default:
          throw new ArgumentException($"An unrecognized texture format was provided: {format}", nameof(format));
      }
    }
  }
}