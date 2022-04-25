using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A palette of <see cref="Color"/>s, with span and range support.</summary>
public readonly record struct ColorPalette(Color[] colors, int offset, int count) : IEnumerable<Color>
{
  /// <summary>Loads a <see cref="ColorPalette"/> from the given <see cref="VirtualPath"/>.</summary>
  public static async ValueTask<ColorPalette> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream);

    if (await reader.ReadLineAsync() != "JASC-PAL")
      throw new FormatException($"Failed to recognize the palette file {path}");

    if (await reader.ReadLineAsync() != "0100")
      throw new FormatException($"Failed to recognize the palette file {path}");

    var rawCount = await reader.ReadLineAsync();
    if (rawCount == null)
      throw new FormatException($"Expected a count row in palette file {path}");

    var count = int.Parse(rawCount, CultureInfo.InvariantCulture);
    var colors = new Color[count];

    for (var i = 0; i < colors.Length; i++)
    {
      var line = await reader.ReadLineAsync();
      if (line == null)
        throw new FormatException($"Expected a palette entry in row {i} of palette file {path}");

      var raw = line.Split(' ')
        .Select(_ => _.Trim())
        .Select(byte.Parse)
        .ToArray();

      if (raw.Length != 3)
        throw new FormatException($"Expected 3 but received {raw.Length} color values!");

      colors[i] = new Color32(raw[0], raw[1], raw[2]);
    }

    return new ColorPalette(colors);
  }

  private readonly Color[] colors = colors;
  private readonly int offset = offset;
  private readonly int count = count;

  public ColorPalette(Color[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <summary>The number of colors in the palette.</summary>
  [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
  public int Count => count;

  /// <summary>Accesses a single color in the palette.</summary>
  public Color this[Index index]
  {
    get => colors?[offset + index.GetOffset(Count) % Count] ?? Color.Clear;
  }

  /// <summary>Accesses a sub-range of the <see cref="ColorPalette"/>.</summary>
  public ColorPalette this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return new ColorPalette(colors, offset, length);
    }
  }

  public Enumerator GetEnumerator() => new(this);
  IEnumerator<Color> IEnumerable<Color>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public static implicit operator ColorPalette(Color[] colors) => new(colors, 0, colors.Length);

  /// <summary>Enumerates the <see cref="ColorPalette"/>.</summary>
  public struct Enumerator : IEnumerator<Color>
  {
    private readonly ColorPalette palette;
    private int index;

    public Enumerator(ColorPalette palette)
    {
      this.palette = palette;
      index        = -1;
    }

    public Color       Current => palette[index];
    object IEnumerator.Current => Current;

    public bool MoveNext() => ++index < palette.Count;
    public void Reset() => index = -1;

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ColorPalette"/>s.s</summary>
public sealed class ColorPaletteLoader : AssetLoader<ColorPalette>
{
  public override ValueTask<ColorPalette> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    return ColorPalette.LoadAsync(context.Path, cancellationToken);
  }
}
