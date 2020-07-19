using System.Collections;
using System.Collections.Generic;
using Surreal.Collections;

namespace Isaac.Core.Items {
  public sealed class Inventory : IEnumerable<ItemStack> {
    private readonly Bag<ItemStack> stacks = new Bag<ItemStack>();

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

    private ref ItemStack GetStack(Item item) {
      for (var i = 0; i < stacks.Count; i++) {
        if (stacks[i].Item == item) {
          return ref stacks.Get(i);
        }
      }

      stacks.Add(new ItemStack(item, count: 0));

      return ref GetStack(item);
    }

    public Bag<ItemStack>.Enumerator              GetEnumerator() => stacks.GetEnumerator();
    IEnumerator<ItemStack> IEnumerable<ItemStack>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
  }
}