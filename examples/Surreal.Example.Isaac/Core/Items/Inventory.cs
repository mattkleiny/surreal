using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Surreal.Collections;

namespace Isaac.Core.Items {
  [DebuggerDisplay("Inventory ({stacks.Count} stacks)")]
  public sealed class Inventory : IEnumerable<ItemStack> {
    private readonly Bag<ItemStack> stacks = new();

    public int this[Item item] {
      get => GetStack(item).Count;
      set {
        ref var stack = ref GetStack(item);

        stack.Count = value;

        if (stack.Count <= 0) {
          stacks.Remove(stack);
        }
      }
    }

    public void Clear() => stacks.Clear();

    private ref ItemStack GetStack(Item item) {
      for (var i = 0; i < stacks.Count; i++) {
        if (stacks[i].Item == item) {
          return ref stacks.Get(i);
        }
      }

      stacks.Add(new(item, count: 0));

      return ref GetStack(item);
    }

    public void Save(BinaryWriter writer) {
      writer.Write(stacks.Count);

      for (var i = 0; i < stacks.Count; i++) {
        var stack = stacks[i];

        writer.Write(stack.Item.Id);
        writer.Write(stack.Count);
      }
    }

    public void Load(BinaryReader reader) {
      Clear();

      var count = reader.ReadInt32();

      for (var i = 0; i < count; i++) {
        var id     = reader.ReadUInt16();
        var amount = reader.ReadInt32();

        this[Item.Palette[id]] = amount;
      }
    }

    public Bag<ItemStack>.Enumerator              GetEnumerator() => stacks.GetEnumerator();
    IEnumerator<ItemStack> IEnumerable<ItemStack>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
  }
}