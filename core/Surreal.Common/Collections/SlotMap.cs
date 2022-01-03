using System.Runtime.CompilerServices;

namespace Surreal.Collections;

/// <summary>
/// A collection with persistent unique keys for stored values.
/// Upon insertion a key is returned that can be used to later access or remove the values.
/// Insertion, removal and access all take O(1) time with low overhead.
/// </summary>
public sealed class SlotMap<T>
{
  private readonly List<Item> items    = new();
  private readonly Queue<int> freeList = new();
  private          int        version  = 0;

  public T Get(ulong key)
  {
    if (!TryGet(key, out var obj))
      throw new ObjectNotFoundException();

    return obj;
  }

  public bool TryGet(ulong key, out T result)
  {
    result = default;

    SplitKey(key, out var index, out var version);

    if (index >= items.Count)
      return false;

    var item = items[index];

    if (item.Version != version)
      return false;

    result = item.Value;
    return true;
  }

  public ulong Add(T obj)
  {
    int index;
    int version;

    if (freeList.Count == 0)
    {
      items.Add(new Item
      {
        Value   = obj,
        Version = this.version,
      });

      index   = items.Count - 1;
      version = this.version++;
    }
    else
    {
      index   = freeList.Dequeue();
      version = this.version++;

      items[index] = new Item
      {
        Value   = obj,
        Version = version,
      };
    }

    return MakeKey(index, version);
  }

  public void Remove(ulong key)
  {
    if (!TryRemove(key))
      throw new ObjectNotFoundException();
  }

  public bool TryRemove(ulong key)
  {
    SplitKey(key, out var index, out var version);

    if (index >= items.Count)
      return false;

    var item = items[index];

    if (item.Version != version)
      return false;

    freeList.Enqueue(index);
    item.Version = -1;
    item.Value   = default;
    items[index] = item;

    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ulong MakeKey(int index, int version) => (ulong) version << 32 | (uint) index;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void SplitKey(ulong key, out int index, out int version)
  {
    index   = (int) (key & 0xFFFFFFFF);
    version = (int) (key >> 32);
  }

  private struct Item
  {
    public T   Value;
    public int Version;
  }
}

/// <summary>When an item is not found in the <see cref="SlotMap{T}"/>.</summary>
public sealed class ObjectNotFoundException : Exception
{
  public ObjectNotFoundException()
    : this("The object was not found in the slotmap")
  {
  }

  public ObjectNotFoundException(string message)
    : base(message)
  {
  }

  public ObjectNotFoundException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
