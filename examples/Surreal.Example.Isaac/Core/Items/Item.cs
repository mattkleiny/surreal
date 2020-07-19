using Surreal.Framework.Palettes;

namespace Isaac.Core.Items {
  public sealed class Item : IHasId {
    public static readonly Item Bomb = new Item(id: 0, name: "Bomb");
    public static readonly Item Key  = new Item(id: 1, name: "Key");

    public static readonly IPalette<Item> Palette = new StaticPalette<Item>();

    public Item(ushort id, string name) {
      Id   = id;
      Name = name;
    }

    public ushort Id   { get; }
    public string Name { get; }
  }
}