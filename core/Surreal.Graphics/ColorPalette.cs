using System.Globalization;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A palette of <see cref="Color32" />s, with span and range support.</summary>
public sealed record ColorPalette(Color32[] _colors, int Offset, int Count) : IReadOnlyList<Color32>
{
  private readonly Color32[] _colors = _colors;

  public ColorPalette(Color32[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <summary>Allows accessing the palette as a <see cref="Span{T}" />.</summary>
  public ReadOnlySpan<Color32> Span => _colors;

  /// <summary>Accesses a single color of the <see cref="ColorPalette" />.</summary>
  public Color32 this[Index index] => _colors[Offset + index.GetOffset(Count)];

  /// <summary>Accesses a sub-range of the <see cref="ColorPalette" />.</summary>
  public ColorPalette this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return new ColorPalette(_colors, offset, length);
    }
  }

  /// <summary>Accesses a single color of the <see cref="ColorPalette" />.</summary>
  public Color32 this[int index] => _colors[Offset + index];

  IEnumerator<Color32> IEnumerable<Color32>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  /// <summary>Enumerates the <see cref="ColorPalette" />.</summary>
  public struct Enumerator : IEnumerator<Color32>
  {
    private readonly ColorPalette _palette;
    private int _index;

    public Enumerator(ColorPalette palette)
    {
      _palette = palette;
      _index = -1;
    }

    public Color32 Current => _palette[_index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return ++_index < _palette.Count;
    }

    public void Reset()
    {
      _index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>The <see cref="AssetLoader{T}" /> for <see cref="ColorPalette" />s.s</summary>
public sealed class ColorPaletteLoader : AssetLoader<ColorPalette>
{
  public override async Task<ColorPalette> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream);

    if (await reader.ReadLineAsync() != "JASC-PAL")
    {
      throw new FormatException($"Failed to recognize the palette file {context.Path}");
    }

    if (await reader.ReadLineAsync() != "0100")
    {
      throw new FormatException($"Failed to recognize the palette file {context.Path}");
    }

    var rawCount = await reader.ReadLineAsync();
    if (rawCount == null)
    {
      throw new FormatException($"Expected a count row in palette file {context.Path}");
    }

    var count = int.Parse(rawCount, CultureInfo.InvariantCulture);
    var colors = new Color32[count];

    for (var i = 0; i < colors.Length; i++)
    {
      var line = await reader.ReadLineAsync();
      if (line == null)
      {
        throw new FormatException($"Expected a palette entry in row {i} of palette file {context.Path}");
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
}





