namespace Surreal.Collections;

/// <summary>
/// A safe index into an item in an <see cref="Arena{T}" />.
/// </summary>
public readonly record struct ArenaIndex(ushort Id, uint Generation)
{
  public static ArenaIndex Invalid => default;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ArenaIndex FromUlong(ulong value)
  {
    return new ArenaIndex((ushort)value, (uint)(value >> 32));
  }

  public bool IsInvalid => Id == 0;
  public bool IsValid => Id != 0;

  public int Offset => Id - 1;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ulong(ArenaIndex index)
  {
    return ((ulong)index.Generation << 32) | index.Id;
  }
}

/// <summary>
/// A generational arena is a collection of <see cref="T" /> with safe externalized indices in the form of (<see cref="ArenaIndex" />).
/// </summary>
public sealed class Arena<T> : IEnumerable<T>
  where T : notnull
{
  private Entry[] _entries = [];
  private int _count;
  private uint _generation = 1;
  private bool _hasDirtyGeneration;
  private uint _nextIndex;

  /// <summary>
  /// The number of items in the arena.
  /// </summary>
  public int Count => _count;

  /// <summary>
  /// Gets the value at the given arena index.
  /// </summary>
  public T this[ArenaIndex index]
  {
    get
    {
      if (!TryGet(index, out var entry))
      {
        throw new InvalidOperationException("The given arena index is not valid.");
      }

      return entry;
    }
  }

  /// <summary>
  /// Determines if the given index is contained in the arena.
  /// </summary>
  public bool Contains(ArenaIndex index)
  {
    var offset = index.Offset;

    if (offset >= _entries.Length)
    {
      return false;
    }

    return index.Generation == _entries[offset].Generation;
  }

  /// <summary>
  /// Attempts to get the entry with index <see cref="index"/>.
  /// </summary>
  public bool TryGet(ArenaIndex index, [MaybeNullWhen(false)] out T result)
  {
    var offset = index.Offset;

    if (offset >= _entries.Length)
    {
      result = default;
      return false;
    }

    ref var entry = ref _entries[offset];

    if (entry.Generation != index.Generation)
    {
      result = default;
      return false;
    }

    result = entry.Value;
    return true;
  }

  /// <summary>
  /// Adds a new item to the arena and returns its <see cref="ArenaIndex" />.
  /// </summary>
  public ArenaIndex Add(T value)
  {
    var index = AllocateIndex();

    _entries[index.Offset] = new Entry(value, index.Generation);
    _count += 1;

    return index;
  }

  /// <summary>
  /// Removes the item at the given index from the arena.
  /// </summary>
  public void Remove(ArenaIndex index)
  {
    ref var entry = ref _entries[index.Id - 1];

    if (entry.Generation == index.Generation)
    {
      entry = default;

      _count -= 1;
      _hasDirtyGeneration = true;
    }
  }

  /// <summary>
  /// Attempts to remove the item at the given index from the arena.
  /// </summary>
  public bool TryRemove(ArenaIndex index, [MaybeNullWhen(false)] out T result)
  {
    ref var entry = ref _entries[index.Id - 1];

    if (entry.Generation == index.Generation)
    {
      result = entry.Value;
      entry = default;

      _count -= 1;
      _hasDirtyGeneration = true;

      return true;
    }

    result = default;
    return false;
  }

  /// <summary>
  /// Clears the arena of all items.
  /// </summary>
  public void Clear()
  {
    _entries = [];
    _count = 0;
    _nextIndex = 0;
    _hasDirtyGeneration = true;
  }

  /// <summary>
  /// Compacts the arena by removing all empty entries that are no longer valid.
  /// </summary>
  public void Compact()
  {
    var newEntries = new Entry[_count];
    var newCount = 0;

    for (var i = 0; i < _entries.Length; i++)
    {
      ref var entry = ref _entries[i];

      if (entry.IsOccupied)
      {
        newEntries[newCount++] = entry;
      }
    }

    _entries = newEntries;
    _count   = newCount;
  }

  /// <summary>
  /// Allocates a new <see cref="ArenaIndex" /> either by finding the first free spot in the list or allocating new space.
  /// </summary>
  private ArenaIndex AllocateIndex()
  {
    // bump the generation when we need to allocate and we've since removed an item
    if (_hasDirtyGeneration)
    {
      Interlocked.Increment(ref _generation);

      _hasDirtyGeneration = false;
    }

    // try and re-use an existing index
    for (var i = 0; i < _entries.Length; i++)
    {
      ref var entry = ref _entries[i];

      if (!entry.IsOccupied && entry.Generation != _generation)
      {
        return new ArenaIndex((ushort)(i + 1), _generation);
      }
    }

    // otherwise allocate a new one and make space if necessary
    var id = (ushort)Interlocked.Increment(ref _nextIndex);
    if (id >= _entries.Length)
    {
      Array.Resize(ref _entries, id);
    }

    return new ArenaIndex(id, _generation);
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Manages a single entry in the <see cref="Arena{T}" />.
  /// </summary>
  [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
  private record struct Entry(T Value, uint Generation)
  {
    /// <summary>
    /// The value of the entry.
    /// </summary>
    public T Value = Value;

    /// <summary>
    /// True if the entry is occupied.
    /// </summary>
    public bool IsOccupied = true;

    /// <summary>
    /// The generation that the entry was created in.
    /// </summary>
    public uint Generation = Generation;
  }

  /// <summary>
  /// Allows enumerating an <see cref="Arena{T}" />.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Arena<T> _arena;
    private int _index;

    public Enumerator(Arena<T> arena)
      : this()
    {
      _arena = arena;
      Reset();
    }

    public ref T Current => ref _arena._entries[_index].Value;
    T IEnumerator<T>.Current => _arena._entries[_index].Value;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      while (++_index < _arena._entries.Length)
      {
        if (_arena._entries[_index].IsOccupied)
        {
          return true;
        }
      }

      return false;
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
