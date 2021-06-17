namespace Surreal.Graphics.Sprites {
  public interface IPlannedSpriteAnimation {
    int  CellCount       { get; }
    bool IsLooping       { get; set; }
    bool IsCycling       { get; set; }
    int  FramesPerSecond { get; set; }

    IPlannedSprite this[int index] { get; }
  }
}