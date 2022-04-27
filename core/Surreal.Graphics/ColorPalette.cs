using System.Globalization;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A palette of <see cref="Color"/>s, with span and range support.</summary>
public sealed record ColorPalette(Color[] colors, int Offset, int Count) : IReadOnlyList<Color>
{
  /// <summary>Loads a <see cref="ColorPalette"/> from the given <see cref="VirtualPath"/>.</summary>
  public static async ValueTask<ColorPalette> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream);

    if (await reader.ReadLineAsync() != "JASC-PAL")
    {
      throw new FormatException($"Failed to recognize the palette file {path}");
    }

    if (await reader.ReadLineAsync() != "0100")
    {
      throw new FormatException($"Failed to recognize the palette file {path}");
    }

    var rawCount = await reader.ReadLineAsync();
    if (rawCount == null)
    {
      throw new FormatException($"Expected a count row in palette file {path}");
    }

    var count = int.Parse(rawCount, CultureInfo.InvariantCulture);
    var colors = new Color[count];

    for (var i = 0; i < colors.Length; i++)
    {
      var line = await reader.ReadLineAsync();
      if (line == null)
      {
        throw new FormatException($"Expected a palette entry in row {i} of palette file {path}");
      }

      var raw = line.Split(' ')
        .Select(_ => _.Trim())
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

  private readonly Color[] colors = colors;

  public ColorPalette(Color[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <summary>Accesses a single color of the <see cref="ColorPalette"/>.</summary>
  public Color this[int index] => colors[Offset + index];

  /// <summary>Accesses a single color of the <see cref="ColorPalette"/>.</summary>
  public Color this[Index index] => colors[Offset + index.GetOffset(Count)];

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
  public override ValueTask<ColorPalette> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return ColorPalette.LoadAsync(context.Path, cancellationToken);
  }
}
