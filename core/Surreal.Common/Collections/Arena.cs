using System.Globalization;

namespace Surreal.Collections;

// TODO: clean up this -1, +1 nonsense.
// TODO: have a better indexer, maybe use an option type or something?

/// <summary>An index or pointer into an item in an <see cref="Arena{T}"/>.</summary>
public readonly record struct ArenaIndex(ushort Id, uint Generation)
{
  public static ArenaIndex Invalid => default;

  public bool IsInvalid => Id == 0;
  public bool IsValid   => Id != 0;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}

/// <summary>An arena is a collection of objects with safe externalized indices (<see cref="ArenaIndex"/>).</summary>
public sealed class Arena<T> : IEnumerable<T>
  where T : notnull
{
  private static T empty = default!;

  private Entry[] entries = Array.Empty<Entry>();

  private uint nextIndex = 1;
  private uint generation = 1;

  /// <summary>Accesses a single item in the arena.</summary>
  public ref T this[ArenaIndex index]
  {
    get
    {
      ref var entry = ref entries[index.Id - 1];

      if (index.Generation != entry.Generation)
      {
        return ref empty;
      }

      return ref entry.Value;
    }
  }

  /// <summary>Adds a new item to the arena and returns it's <see cref="ArenaIndex"/>.</summary>
  public ArenaIndex Add(T value)
  {
    var index = AllocateIndex();

    // make space if necessary
    if (entries.Length < index.Id)
    {
      Array.Resize(ref entries, index.Id);
    }

    entries[index.Id - 1] = new Entry(value, index.Generation);

    return index;
  }

  /// <summary>Removes the item at the given index from the arena.</summary>
  public void Remove(ArenaIndex index)
  {
    ref var entry = ref entries[index.Id - 1];

    if (entry.Generation == index.Generation)
    {
      entry = default;

      Interlocked.Increment(ref generation);
    }
  }

  /// <summary>Removes a batch of the given indices from the arena.</summary>
  public void RemoveAll(ReadOnlySpan<ArenaIndex> indices)
  {
    foreach (var index in indices)
    {
      ref var entry = ref entries[index.Id - 1];

      if (entry.Generation == index.Generation)
      {
        entry = default;
      }
    }

    Interlocked.Increment(ref generation);
  }

  /// <summary>Allocates a new <see cref="ArenaIndex"/> either by finding the first free spot in the list or allocating new space.</summary>
  private ArenaIndex AllocateIndex()
  {
    // try and re-use an existing index
    for (var i = 0; i < entries.Length; i++)
    {
      ref var entry = ref entries[i];

      if (!entry.IsOccupied && entry.Generation != generation)
      {
        return new ArenaIndex((ushort)(i + 1), generation);
      }
    }

    // otherwise allocate a new one
    var index = (ushort)(Interlocked.Increment(ref nextIndex) - 1);

    return new ArenaIndex(index, generation);
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

  /// <summary>Allows enumerating an <see cref="Arena{T}"/>.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Arena<T> arena;
    private int index;

    public Enumerator(Arena<T> arena)
      : this()
    {
      this.arena = arena;
      Reset();
    }

    public ref T       Current => ref arena.entries[index].Value;
    T IEnumerator<T>.  Current => arena.entries[index].Value;
    object IEnumerator.Current => Current!;

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

  /// <summary>Manages a single entry in the <see cref="Arena{T}"/>.</summary>
  [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
  private record struct Entry(T Value, uint Generation)
  {
    public T Value = Value;
    public bool IsOccupied = true;
    public uint Generation = Generation;
  }
}
