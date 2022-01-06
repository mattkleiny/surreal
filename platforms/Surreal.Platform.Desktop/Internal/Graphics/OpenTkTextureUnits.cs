using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTkTextureUnits : ITextureUnits
{
  private readonly Texture[] textures;

  public OpenTkTextureUnits(int capacity)
  {
    Debug.Assert(capacity > 0, "capacity > 0");

    textures = new Texture[capacity + 1];
  }

  public Texture? this[int unit]
  {
    get
    {
      Debug.Assert(unit >= 0, "unit >= 0");
      Debug.Assert(unit < textures.Length, "unit < textures.Length");

      return textures[unit];
    }
    set
    {
      Debug.Assert(unit >= 0, "unit >= 0");
      Debug.Assert(unit < textures.Length, "unit < textures.Length");

      var texture = (OpenTkTexture) value!;

      textures[unit] = texture;

      GL.ActiveTexture(SelectUnit(unit));
      GL.BindTexture(TextureTarget.Texture2D, texture.Id);
    }
  }

  private static TextureUnit SelectUnit(int unit)
  {
    switch (unit)
    {
      case 0:  return TextureUnit.Texture0;
      case 1:  return TextureUnit.Texture1;
      case 2:  return TextureUnit.Texture2;
      case 3:  return TextureUnit.Texture3;
      case 4:  return TextureUnit.Texture4;
      case 5:  return TextureUnit.Texture5;
      case 6:  return TextureUnit.Texture6;
      case 7:  return TextureUnit.Texture7;
      case 8:  return TextureUnit.Texture8;
      case 9:  return TextureUnit.Texture9;
      case 10: return TextureUnit.Texture10;

      default:
        throw new ArgumentOutOfRangeException(nameof(unit), unit, "An unsupported texture unit was requested.");
    }
  }
}