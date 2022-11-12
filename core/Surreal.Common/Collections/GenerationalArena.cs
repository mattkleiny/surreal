using System.Globalization;
using System.Runtime.CompilerServices;

namespace Surreal.Collections;

/// <summary>A safe index into an item in an <see cref="GenerationalArena{T}" />.</summary>
public readonly record struct ArenaIndex(ushort Id, uint Generation)
{
  public static ArenaIndex Invalid => default;

  public bool IsInvalid => Id == 0;
  public bool IsValid => Id != 0;

  public int Offset => Id - 1;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}

/// <summary>A generational arena is a collection of <see cref="T" /> with safe externalized indices in the form of (<see cref="ArenaIndex" />).</summary>
public sealed class GenerationalArena<T> : IEnumerable<T>
  where T : notnull
{
  private Entry[] _entries = Array.Empty<Entry>();
  private uint _generation = 1;
  private bool _hasDirtyGeneration;

  private uint _nextIndex = 0;

  // TODO: have a better indexer, maybe use an option type or something?
  /// <summary>Accesses a single item in the arena.</summary>
  public ref T this[ArenaIndex index]
  {
    get
    {
      var offset = index.Offset;

      if (offset < _entries.Length)
      {
        ref var entry = ref _entries[offset];

        if (index.Generation == entry.Generation)
        {
          return ref entry.Value;
        }
      }

      return ref Unsafe.NullRef<T>();
    }
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>Determines if the given index is contained in the arena.</summary>
  public bool Contains(ArenaIndex index)
  {
    var offset = index.Offset;

    if (offset >= _entries.Length)
    {
      return false;
    }

    return index.Generation == _entries[offset].Generation;
  }

  /// <summary>Inserts an entry into the arena with the given fixed <see cref="ArenaIndex" />.</summary>
  public void Insert(ArenaIndex index, T value)
  {
    var offset = index.Offset;

    if (offset >= _entries.Length)
    {
      Array.Resize(ref _entries, offset + 1);
    }

    _entries[offset] = new Entry(value, index.Generation);
  }

  /// <summary>Adds a new item to the arena and returns it's <see cref="ArenaIndex" />.</summary>
  public ArenaIndex Add(T value)
  {
    var index = AllocateIndex();

    _entries[index.Offset] = new Entry(value, index.Generation);

    return index;
  }

  /// <summary>Removes the item at the given index from the arena.</summary>
  public void Remove(ArenaIndex index)
  {
    ref var entry = ref _entries[index.Id - 1];

    if (entry.Generation == index.Generation)
    {
      entry = default;

      _hasDirtyGeneration = true;
    }
  }

  /// <summary>
  ///   Allocates a new <see cref="ArenaIndex" /> either by finding the first free spot in the list
  ///   or allocating new space.
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
        return new ArenaIndex((ushort) (i + 1), _generation);
      }
    }

    // otherwise allocate a new one and make space if necessary
    var id = (ushort) Interlocked.Increment(ref _nextIndex);
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

  /// <summary>Manages a single entry in the <see cref="GenerationalArena{T}" />.</summary>
  [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
  private record struct Entry(T Value, uint Generation)
  {
    public uint Generation = Generation;
    public bool IsOccupied = true;
    public T Value = Value;
  }

  /// <summary>Allows enumerating an <see cref="GenerationalArena{T}" />.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly GenerationalArena<T> _arena;
    private int _index;

    public Enumerator(GenerationalArena<T> arena)
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
        if (_arena._entries[_index].IsOccupied)
        {
          return true;
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

