namespace Minecraft.Core.Generation {
  public interface IChunkView {
    int Width  { get; }
    int Height { get; }
    int Depth  { get; }

    Block this[int x, int y, int z] { get; set; }
  }
}