using Surreal.Graphics;
using Surreal.Graphics.Textures;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessTextureUnits : ITextureUnits
{
  public Texture? this[int unit]
  {
    get => null;
    set
    {
      // no-op
    }
  }
}