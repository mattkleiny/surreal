namespace Isaac.Core.Items {
  public struct ItemStack {
    public readonly Item Item;
    public          int  Count;

    public ItemStack(Item item, int count) {
      Item  = item;
      Count = count;
    }
  }
}