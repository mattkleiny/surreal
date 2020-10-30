using Surreal.Framework.Palettes;

namespace Isaac.Core.Items {
  public sealed record Item(ushort Id, string Name) : IHasId {
    public static readonly Item Bomb = new(Id: 0, Name: "Bomb");
    public static readonly Item Key  = new(Id: 1, Name: "Key");

    public static readonly IPalette<Item> Palette = new StaticPalette<Item>();
  }
}