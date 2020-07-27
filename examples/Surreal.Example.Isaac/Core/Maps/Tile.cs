using Surreal.Framework.Palettes;
using Surreal.Graphics;

namespace Isaac.Core.Maps {
  public sealed class Tile : IHasId {
    public static readonly Tile Empty = new Tile(id: 0) {IsVisible = false};
    public static readonly Tile Wall  = new Tile(id: 1) {Color     = Color.White};
    public static readonly Tile Floor = new Tile(id: 2) {Color     = Color.Green, IsPathable = true};

    public static IPalette<Tile> Palette { get; } = new StaticPalette<Tile>();

    private Tile(ushort id) => Id = id;

    public ushort Id           { get; }
    public bool   IsCollidable { get; set; } = true;
    public bool   IsVisible    { get; set; } = true;
    public bool   IsPathable   { get; set; } = false;
    public Color  Color        { get; set; } = Color.Clear;
  }
}