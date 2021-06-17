namespace Surreal.Framework.Palettes {
  public interface IPalette<TTile> {
    TTile Empty { get; }
    int   Count { get; }

    ushort this[TTile tile] { get; }
    TTile this[ushort id] { get; }
  }
}