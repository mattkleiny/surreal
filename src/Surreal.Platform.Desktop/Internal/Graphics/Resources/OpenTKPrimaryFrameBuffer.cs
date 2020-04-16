using System.Diagnostics;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  [DebuggerDisplay("Primary Render Target")]
  internal sealed class OpenTKPrimaryFrameBuffer : FrameBuffer
  {
    public override Texture Texture { get; } = new OpenTKTexture(
      format: TextureFormat.RGBA8888,
      filterMode: TextureFilterMode.Point,
      wrapMode: TextureWrapMode.Clamp
    );

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        Texture.Dispose();
      }

      base.Dispose(managed);
    }
  }
}
