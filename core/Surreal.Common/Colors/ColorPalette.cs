namespace Surreal.Colors;

/// <summary>
/// A palette of <see cref="Color32" />s, with span and range support.
/// </summary>
public sealed record ColorPalette(Color32[] _colors, int Offset, int Count) : IReadOnlyList<Color32>
{
  private readonly Color32[] _colors = _colors;

  public ColorPalette(Color32[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <summary>
  /// Allows accessing the palette as a <see cref="Span{T}" />.
  /// </summary>
  public ReadOnlySpan<Color32> Span => _colors;

  /// <summary>
  /// Accesses a single color of the <see cref="ColorPalette" />.
  /// </summary>
  public Color32 this[Index index] => _colors[Offset + index.GetOffset(Count)];

  /// <summary>
  /// Accesses a sub-range of the <see cref="ColorPalette" />.
  /// </summary>
  public ColorPalette this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return new ColorPalette(_colors, offset, length);
    }
  }

  /// <summary>
  /// Accesses a single color of the <see cref="ColorPalette" />.
  /// </summary>
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

  /// <summary>
  /// Enumerates the <see cref="ColorPalette" />.
  /// </summary>
  public struct Enumerator(ColorPalette palette) : IEnumerator<Color32>
  {
    private readonly ColorPalette _palette = palette;
    private int _index = -1;

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
