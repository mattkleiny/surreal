namespace Surreal;

/// <summary>
/// A stack-only type with the ability to rent a buffer of a specified length and getting a <see cref="Span{T}"/> from it.
/// This type mirrors <see cref="MemoryOwner{T}"/> but without allocations and with further optimizations.
/// As this is a stack-only type, it relies on the duck-typed <see cref="IDisposable"/> pattern introduced with C# 8.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public readonly ref struct SpanOwner<T>
{
  public static SpanOwner<T> Empty
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => new(0, ArrayPool<T>.Shared);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static SpanOwner<T> Allocate(int size, bool zeroFill = false) => new(size, ArrayPool<T>.Shared, zeroFill);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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