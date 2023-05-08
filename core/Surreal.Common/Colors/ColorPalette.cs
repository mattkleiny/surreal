namespace Surreal.Colors;

/// <summary>
/// A palette of <see cref="ColorB" />s, with span and range support.
/// </summary>
public sealed record ColorPalette(ColorB[] _colors, int Offset, int Count) : IReadOnlyList<ColorB>
{
  private readonly ColorB[] _colors = _colors;

  public ColorPalette(ColorB[] colors)
    : this(colors, 0, colors.Length)
  {
  }

  /// <summary>
  /// Allows accessing the palette as a <see cref="Span{T}" />.
  /// </summary>
  public ReadOnlySpan<ColorB> Span => _colors;

  /// <summary>
  /// Accesses a single color of the <see cref="ColorPalette" />.
  /// </summary>
  public ColorB this[Index index] => _colors[Offset + index.GetOffset(Count)];

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
  public ColorB this[int index] => _colors[Offset + index];

  IEnumerator<ColorB> IEnumerable<ColorB>.GetEnumerator()
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
  public struct Enumerator : IEnumerator<ColorB>
  {
    private readonly ColorPalette _palette;
    private int _index;

    public Enumerator(ColorPalette palette)
    {
      _palette = palette;
      _index = -1;
    }

    public ColorB Current => _palette[_index];
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
