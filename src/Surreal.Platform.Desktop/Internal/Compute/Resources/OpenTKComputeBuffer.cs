using System;
using System.Runtime.CompilerServices;
using Surreal.Compute.Memory;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Compute.Resources
{
  internal sealed class OpenTKComputeBuffer : ComputeBuffer
  {
    private readonly Pixmap        pixmap;
    private readonly OpenTKTexture texture;

    public OpenTKComputeBuffer(int width, int height)
      : base(Unsafe.SizeOf<byte>())
    {
      // we're basically an OpenGL image with random access
      pixmap  = new Pixmap(width, height);
      texture = new OpenTKTexture(pixmap.Format, TextureFilterMode.Point, TextureWrapMode.Clamp);
    }

    public override void Put<T>(Span<T> data)
    {
      data.Cast<T, Color>().CopyTo(pixmap.Span);

      texture.Upload(pixmap);
    }

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        texture.Dispose();
        pixmap.Dispose();
      }

      base.Dispose(managed);
    }
  }
}