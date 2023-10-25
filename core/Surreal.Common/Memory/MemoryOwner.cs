namespace Surreal.Memory;

/// <summary>
/// A <see cref="IMemoryOwner{T}"/> implementation with conveniences for fast access.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public sealed class MemoryOwner<T> : IMemoryOwner<T>
{
  /// <summary>
  /// An empty <see cref="MemoryOwner{T}"/> instance.
  /// </summary>
  public static MemoryOwner<T> Empty => new(0, ArrayPool<T>.Shared);

  public static MemoryOwner<T> Allocate(int size, bool zeroFill = false) => new(size, ArrayPool<T>.Shared, zeroFill);
  public static MemoryOwner<T> Allocate(int size, ArrayPool<T> pool, bool zeroFill = false) => new(size, pool, zeroFill);

  private readonly int _start;
  private readonly int _length;
  private readonly ArrayPool<T> _pool;
  private T[]? _array;

  private MemoryOwner(int length, ArrayPool<T> pool, bool zeroFill = false)
  {
    _start = 0;
    _length = length;
    _pool = pool;
    _array = pool.Rent(length);

    if (zeroFill)
    {
      _array.AsSpan(0, length).Clear();
    }
  }

  private MemoryOwner(int start, int length, ArrayPool<T> pool, T[] array)
  {
    _start = start;
    _length = length;
    _pool = pool;
    _array = array;
  }

  /// <summary>
  /// The length of the current buffer.
  /// </summary>
  public int Length => _length;

  /// <inheritdoc/>
  public Memory<T> Memory
  {
    get
    {
      if (_array is null)
      {
        throw new ObjectDisposedException(nameof(MemoryOwner<T>), "The current buffer has already been disposed");
      }

      return new Memory<T>(_array, _start, _length);
    }
  }

  /// <summary>
  /// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
  /// </summary>
  public Span<T> Span
  {
    get
    {
      if (_array is null)
      {
        throw new ObjectDisposedException(nameof(MemoryOwner<T>), "The current buffer has already been disposed");
      }

      ref T r0 = ref MemoryMarshal.GetArrayDataReference(_array);
      ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)_start);

      return MemoryMarshal.CreateSpan(ref ri, _length);
    }
  }

  /// <summary>
  /// Slices the buffer currently in use and returns a new <see cref="MemoryOwner{T}"/> instance.
  /// </summary>
  public MemoryOwner<T> Slice(int start, int length)
  {
    Debug.Assert((uint)start <= (uint)_length);
    Debug.Assert((uint)length <= (uint)(_length - start));

    if (_array is null)
    {
      throw new ObjectDisposedException(nameof(MemoryOwner<T>), "The current buffer has already been disposed");
    }

    return new MemoryOwner<T>(start, length, _pool, _array);
  }

  public void Dispose()
  {
    if (_array is null)
    {
      return;
    }

    _pool.Return(_array);
    _array = null;
  }

  public override string ToString()
  {
    if (typeof(T) == typeof(char) && _array is char[] chars)
    {
      return new string(chars, _start, _length);
    }

    return $"MemoryOwner<{typeof(T)}>[{_length}]";
  }
}
