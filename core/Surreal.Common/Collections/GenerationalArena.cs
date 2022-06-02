using System.Globalization;
using System.Runtime.CompilerServices;

namespace Surreal.Collections;

/// <summary>A safe index into an item in an <see cref="GenerationalArena{T}"/>.</summary>
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

/// <summary>A generational arena is a collection of <see cref="T"/> with safe externalized indices in the form of (<see cref="ArenaIndex"/>).</summary>
public sealed class GenerationalArena<T> : IEnumerable<T>
  where T : notnull
{
  private Entry[] entries = Array.Empty<Entry>();

  private uint nextIndex = 0;
  private uint generation = 1;
  private bool hasDirtyGeneration;

  // TODO: have a better indexer, maybe use an option type or something?
  /// <summary>Accesses a single item in the arena.</summary>
  public ref T this[ArenaIndex index]
  {
    get
    {
      var offset = index.Offset;

      if (offset < entries.Length)
      {
        ref var entry = ref entries[offset];

        if (index.Generation == entry.Generation)
        {
          return ref entry.Value;
        }
      }

      return ref Unsafe.NullRef<T>();
    }
  }

  /// <summary>Determines if the given index is contained in the arena.</summary>
  public bool Contains(ArenaIndex index)
  {
    var offset = index.Offset;

    if (offset >= entries.Length)
    {
      return false;
    }

    return index.Generation == entries[offset].Generation;
  }

  /// <summary>Inserts an entry into the arena with the given fixed <see cref="ArenaIndex"/>.</summary>
  public void Insert(ArenaIndex index, T value)
  {
    var offset = index.Offset;

    if (offset >= entries.Length)
    {
      Array.Resize(ref entries, offset + 1);
    }

    entries[offset] = new Entry(value, index.Generation);
  }

  /// <summary>Adds a new item to the arena and returns it's <see cref="ArenaIndex"/>.</summary>
  public ArenaIndex Add(T value)
  {
    var index = AllocateIndex();

    entries[index.Offset] = new Entry(value, index.Generation);

    return index;
  }

  /// <summary>Removes the item at the given index from the arena.</summary>
  public void Remove(ArenaIndex index)
  {
    ref var entry = ref entries[index.Id - 1];

    if (entry.Generation == index.Generation)
    {
      entry = default;

      hasDirtyGeneration = true;
    }
  }

  /// <summary>
  /// Allocates a new <see cref="ArenaIndex"/> either by finding the first free spot in the list
  /// or allocating new space.
  /// </summary>
  private ArenaIndex AllocateIndex()
  {
    // bump the generation when we need to allocate and we've since removed an item
    if (hasDirtyGeneration)
    {
      Interlocked.Increment(ref generation);

      hasDirtyGeneration = false;
    }

    // try and re-use an existing index
    for (var i = 0; i < entries.Length; i++)
    {
      ref var entry = ref entries[i];

      if (!entry.IsOccupied && entry.Generation != generation)
      {
        return new ArenaIndex((ushort)(i + 1), generation);
      }
    }

    // otherwise allocate a new one and make space if necessary
    var id = (ushort)(Interlocked.Increment(ref nextIndex));
    if (id >= entries.Length)
    {
      Array.Resize(ref entries, id);
    }

    return new ArenaIndex(id, generation);
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

  /// <summary>Manages a single entry in the <see cref="GenerationalArena{T}"/>.</summary>
  [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
  private record struct Entry(T Value, uint Generation)
  {
    public T Value = Value;
    public bool IsOccupied = true;
    public uint Generation = Generation;
  }

  /// <summary>Allows enumerating an <see cref="GenerationalArena{T}"/>.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly GenerationalArena<T> arena;
    private int index;

    public Enumerator(GenerationalArena<T> arena)
      : this()
    {
      this.arena = arena;
      Reset();
    }

    public ref T Current => ref arena.entries[index].Value;
    T IEnumerator<T>.Current => arena.entries[index].Value;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      while (++index < arena.entries.Length)
      {
        if (arena.entries[index].IsOccupied)
        {
          return true;
        }
      }

      return false;
    }

    public void Reset()
    {
      index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
