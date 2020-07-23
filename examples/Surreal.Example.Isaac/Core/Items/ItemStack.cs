using System.Diagnostics;

namespace Isaac.Core.Items {
  [DebuggerDisplay("{Count} x {Item.Name}")]
  public struct ItemStack {
    public readonly Item Item;
    public          int  Count;

    public ItemStack(Item item, int count) {
      Item  = item;
      Count = count;
    }
  }
}