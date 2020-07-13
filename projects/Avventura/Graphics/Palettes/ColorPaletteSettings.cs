using System;
using System.Runtime.CompilerServices;
using Surreal.Graphics;

namespace Avventura.Graphics.Palettes {
  public readonly struct ColorPaletteSettings : IIndexedColorProvider {
    public static readonly ColorPaletteSettings Default = new ColorPaletteSettings(
        primary: new ColorPalette(Color.White, Color.White, Color.White, Color.White),
        shadow: new ColorPalette(Color.Black, Color.Black, Color.Black, Color.Black),
        userInterface: new ColorPalette(Color.Blue, Color.Blue, Color.Blue, Color.Blue)
    );

    public ColorPalette Primary       { get; }
    public ColorPalette Shadow        { get; }
    public ColorPalette UserInterface { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorPaletteSettings Lerp(ColorPaletteSettings a, ColorPaletteSettings b, float t) => new ColorPaletteSettings(
        primary: ColorPalette.Lerp(a.Primary, b.Primary, t),
        shadow: ColorPalette.Lerp(a.Shadow, b.Shadow, t),
        userInterface: ColorPalette.Lerp(a.UserInterface, b.UserInterface, t)
    );

    public ColorPaletteSettings(ColorPalette primary, ColorPalette shadow, ColorPalette userInterface) {
      Primary       = primary;
      Shadow        = shadow;
      UserInterface = userInterface;
    }

    public ColorPalette this[ColorPaletteChannel channel] => channel switch {
        ColorPaletteChannel.Primary       => Primary,
        ColorPaletteChannel.Shadow        => Shadow,
        ColorPaletteChannel.UserInterface => UserInterface,

        _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
    };
  }
}