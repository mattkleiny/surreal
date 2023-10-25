namespace Surreal.Memory;

/// <summary>
/// A <see cref="IMemoryOwner{T}"/>-like stack-only implementation with conveniences for fast access.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public readonly ref struct SpanOwner<T>
{
  public static SpanOwner<T> Empty => new(0, ArrayPool<T>.Shared);
  public static SpanOwner<T> Allocate(int size, bool zeroFill = false) => new(size, ArrayPool<T>.Shared, zeroFill);
  public static SpanOwner<T> Allocate(int size, ArrayPool<T> pool, bool zeroFill = false) => new(size, pool, zeroFill);

  private readonly int _length;
  private readonly ArrayPool<T> _pool;
  private readonly T[] _array;

  private SpanOwner(int length, ArrayPool<T> pool, bool zeroFill = false)
  {
    _length = length;
    _pool = pool;
    _array = pool.Rent(length);

    if (zeroFill)
    {
      _array.AsSpan(0, length).Clear();
    }
  }

  /// <summary>
  /// The length of the current buffer.
  /// </summary>
  public int Length => _length;

  /// <summary>
  /// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
  /// </summary>
  public Span<T> Span
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => MemoryMarshal.CreateSpan(ref _array[0], _length);
  }

  /// <summary>
  /// Implements the duck-typed <see cref="IDisposable.Dispose"/> method.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Dispose()
  {
    _pool.Return(_array);
  }

  /// <inheritdoc/>
  public override string ToString()
  {
    if (typeof(T) == typeof(char) && _array is char[] chars)
    {
      return new string(chars, 0, _length);
    }

    return $"SpanOwner<{typeof(T)}>[{_length}]";
  }
}
