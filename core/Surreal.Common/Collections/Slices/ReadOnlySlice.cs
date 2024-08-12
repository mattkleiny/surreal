namespace Surreal.Collections.Slices;

/// <summary>
/// Helpers for creating <see cref="ReadOnlySlice{T}" /> instances.
/// </summary>
public static class ReadOnlySlice
{
  /// <summary>
  /// Builds a read-only slice from the given items.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> Build<T>(ReadOnlySpan<T> items) 
    => new(items.ToArray());
}

/// <summary>
/// A read-only <see cref="Slice{T}" /> variant.
/// </summary>
[DebuggerDisplay("ReadOnlySlice ({Length} items)")]
[CollectionBuilder(typeof(ReadOnlySlice), nameof(ReadOnlySlice.Build))]
public readonly struct ReadOnlySlice<T> : IEnumerable<T>
{
  public static ReadOnlySlice<T> Empty => default;

  private readonly IReadOnlyList<T> _list;

  public ReadOnlySlice(IReadOnlyList<T> list)
    : this(list, 0, list.Count)
  {
  }

  public ReadOnlySlice(IReadOnlyList<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0, "offset >= 0");
    Debug.Assert(length >= 0, "length >= 0");

    _list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index] => _list[Offset + index.GetOffset(Length)];

  public ReadOnlySlice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new ReadOnlySlice<T>(_list, offset, length);
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
  public static implicit operator ReadOnlySlice<T>(T[] array) => new(array);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ReadOnlySlice<T>(List<T> list) => new(list);

  /// <summary>
  /// An enumerator for <see cref="ReadOnlySlice{T}" />.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly ReadOnlySlice<T> _slice;
    private int _index;

    public Enumerator(ReadOnlySlice<T> slice)
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