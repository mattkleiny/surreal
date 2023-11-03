using Surreal.Assets;

namespace Surreal.Colors;

/// <summary>
/// A palette of <see cref="Color32" />s, with span and range support.
/// </summary>
[AssetType("434985b1-3325-4e10-b1d9-634b8ff04df5")]
public sealed class ColorPalette(Color32[] colors, int offset, int count) : IReadOnlyList<Color32>
{
  public ColorPalette(Color32[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <inheritdoc/>
  public int Count { get; } = count;

  /// <summary>
  /// Allows accessing the palette as a <see cref="Span{T}" />.
  /// </summary>
  public ReadOnlySpan<Color32> Span => colors;

  /// <summary>
  /// Accesses a single color of the <see cref="ColorPalette" />.
  /// </summary>
  public Color32 this[int index] => colors[offset + index];

  /// <summary>
  /// Accesses a single color of the <see cref="ColorPalette" />.
  /// </summary>
  public Color32 this[Index index] => colors[offset + index.GetOffset(Count)];

  /// <summary>
  /// Accesses a sub-range of the <see cref="ColorPalette" />.
  /// </summary>
  public ColorPalette this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return new ColorPalette(colors, offset, length);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator<Color32> IEnumerable<Color32>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Enumerates the <see cref="ColorPalette" />.
  /// </summary>
  public struct Enumerator(ColorPalette palette) : IEnumerator<Color32>
  {
    private int _index = -1;

    public Color32 Current => palette[_index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return ++_index < palette.Count;
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
