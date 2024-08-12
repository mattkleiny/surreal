namespace Surreal.Collections.Slices;

/// <summary>
/// Helpers for working with <see cref="Slice{T}" /> instances.
/// </summary>
public static class Slice
{
  /// <summary>
  /// Builds a <see cref="Slice{T}"/> from the given items.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> Build<T>(ReadOnlySpan<T> items) 
    => new([..items.ToArray()]);
}

/// <summary>
/// A slice of a <see cref="List{T}" />.
/// </summary>
[DebuggerDisplay("Slice ({Length} items)")]
[CollectionBuilder(typeof(Slice), nameof(Slice.Build))]
public readonly struct Slice<T> : IEnumerable<T>
{
  public static Slice<T> Empty => default;

  private readonly List<T> _list;

  public Slice(List<T> list)
    : this(list, 0, list.Count)
  {
  }

  public Slice(List<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0);
    Debug.Assert(length >= 0);

    _list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index]
  {
    get => _list[Offset + index.GetOffset(Length)];
    set => _list[Offset + index.GetOffset(Length)] = value;
  }

  public Slice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new Slice<T>(_list, offset, length);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Slice<T>(List<T> list) => new(list);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ReadOnlySlice<T>(Slice<T> slice) => new(slice._list, slice.Offset, slice.Length);

  /// <summary>
  /// An enumerator for a <see cref="Slice{T}" />.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Slice<T> _slice;
    private int _index;

    public Enumerator(Slice<T> slice)
      : this()
    {
      _slice = slice;
      Reset();
    }

    public T Current => _slice[_index];
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      return ++_index < _slice.Length;
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
