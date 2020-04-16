using System.Runtime.CompilerServices;
using Surreal.Graphics;

namespace Avventura.Graphics.Palettes
{
  public readonly struct ColorPalette
  {
    public Color Color1 { get; }
    public Color Color2 { get; }
    public Color Color3 { get; }
    public Color Color4 { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorPalette Lerp(ColorPalette a, ColorPalette b, float t) => new ColorPalette(
      color1: Color.Lerp(a.Color1, b.Color1, t),
      color2: Color.Lerp(a.Color2, b.Color2, t),
      color3: Color.Lerp(a.Color3, b.Color3, t),
      color4: Color.Lerp(a.Color4, b.Color4, t)
    );

    public ColorPalette(Color color1, Color color2, Color color3, Color color4)
    {
      Color1 = color1;
      Color2 = color2;
      Color3 = color3;
      Color4 = color4;
    }
  }
}