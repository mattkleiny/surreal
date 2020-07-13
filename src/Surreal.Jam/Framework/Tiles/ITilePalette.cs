namespace Surreal.Framework.Tiles {
  public interface ITilePalette<TTile> {
    int Count { get; }

    ushort this[TTile tile] { get; }
    TTile this[ushort id] { get; }
  }
}