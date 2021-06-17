using System.Collections.Generic;

namespace Surreal.Framework.Scenes.Entities.Collections {
  internal sealed class SlotMap<T> {
    private readonly List<Entry> items;
    private readonly Queue<int>  freeList;

    private int version;

    public SlotMap(int initialCapacity = 16) {
      items    = new List<Entry>(initialCapacity);
      freeList = new Queue<int>(initialCapacity);
    }

    public T this[SlotMapKey key] => Get(key);

    public SlotMapKey Add(T value) {
      int index;
      int version;

      if (freeList.Count == 0) {
        items.Add(new Entry {
          Value   = value,
          Version = this.version,
        });

        index   = items.Count - 1;
        version = this.version++;
      }
      else {
        index   = freeList.Dequeue();
        version = this.version++;

        items[index] = new Entry {
          Value   = value,
          Version = version,
        };
      }

      return new SlotMapKey(index, version);
    }

    public T Get(SlotMapKey key) {
      if (!TryGet(key, out var value)) {
        throw new InvalidSlotException($"The key {key} is not present in the slot map");
      }

      return value;
    }

    public bool TryGet(SlotMapKey key, out T value) {
      value = default!;

      var (index, version) = key;
      if (index >= items.Count) {
        return false;
      }

      var item = items[index];
      if (item.Version != version) {
        return false;
      }

      value = item.Value;
      return true;
    }

    public void Remove(SlotMapKey key) {
      if (!TryRemove(key)) {
        throw new InvalidSlotException($"The key {key} is not present in the slot map");
      }
    }

    public bool TryRemove(SlotMapKey key) {
      var (index, version) = key;
      if (index >= items.Count) {
        return false;
      }

      var item = items[index];
      if (item.Version != version) {
        return false;
      }

      freeList.Enqueue(index);

      item.Version = -1;
      item.Value   = default!;

      items[index] = item;

      return true;
    }

    public bool Contains(SlotMapKey key) {
      var (index, version) = key;
      if (index >= items.Count) {
        return false;
      }

      var item = items[index];
      if (item.Version != version) {
        return false;
      }

      return true;
    }

    private struct Entry {
      public T   Value;
      public int Version;
    }
  }
}