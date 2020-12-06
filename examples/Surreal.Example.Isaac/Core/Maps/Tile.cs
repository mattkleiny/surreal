using Surreal.Framework.Palettes;
using Surreal.Graphics;

namespace Isaac.Core.Maps {
  public sealed record Tile(ushort Id, bool IsCollidable, bool IsVisible, bool IsPathable, Color Color) : IHasId {
    public static readonly Tile Empty = new(Id: 0, IsCollidable: false, IsVisible: false, IsPathable: false, Color: Color.Clear);
    public static readonly Tile Wall  = new(Id: 1, IsCollidable: true, IsVisible: true, IsPathable: false, Color: Color.White);
    public static readonly Tile Floor = new(Id: 2, IsCollidable: false, IsVisible: true, IsPathable: true, Color: Color.Green);

    public static IPalette<Tile> Palette { get; } = new StaticPalette<Tile>();
  }
}