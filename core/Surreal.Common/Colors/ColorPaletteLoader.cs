using Surreal.Assets;
using Surreal.Colors;
using Surreal.IO;

namespace Surreal.Graphics.Utilities;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="ColorPalette" />s.s
/// </summary>
public sealed class ColorPaletteLoader : AssetLoader<ColorPalette>
{
  public override async Task<ColorPalette> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    using var reader = context.Path.OpenInputStreamReader();

    if (await reader.ReadLineAsync(cancellationToken) != "JASC-PAL")
    {
      throw new FormatException($"Failed to recognize the palette file {context.Path}");
    }

    if (await reader.ReadLineAsync(cancellationToken) != "0100")
    {
      throw new FormatException($"Failed to recognize the palette file {context.Path}");
    }

    var rawCount = await reader.ReadLineAsync(cancellationToken);
    if (rawCount == null)
    {
      throw new FormatException($"Expected a count row in palette file {context.Path}");
    }

    var count = int.Parse(rawCount, CultureInfo.InvariantCulture);
    var colors = new Color32[count];

    for (var i = 0; i < colors.Length; i++)
    {
      var line = await reader.ReadLineAsync(cancellationToken);
      if (line == null)
      {
        throw new FormatException($"Expected a palette entry in row {i} of palette file {context.Path}");
      }

      var raw = line.Split(' ')
        .Select(x => x.Trim())
        .Select(byte.Parse)
        .ToArray();

      if (raw.Length != 3)
      {
        throw new FormatException($"Expected 3 but received {raw.Length} color values on row {i}");
      }

      colors[i] = new Color32(raw[0], raw[1], raw[2]);
    }

    return new ColorPalette(colors);
  }
}
