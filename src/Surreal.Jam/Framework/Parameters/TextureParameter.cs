using System.Diagnostics;
using Surreal.Graphics.Textures;

namespace Surreal.Framework.Parameters
{
  [DebuggerDisplay("Texture <{Value}>")]
  public sealed class TextureParameter : Parameter<Texture>
  {
    public TextureParameter(Texture value)
      : base(value)
    {
    }
  }
}
