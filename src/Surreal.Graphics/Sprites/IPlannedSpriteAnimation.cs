namespace Surreal.Graphics.Sprites
{
  public interface IPlannedSpriteAnimation
  {
    bool IsLooping       { get; }
    bool IsCycling       { get; }
    int  FramesPerSecond { get; }
    int  FrameCount      { get; }

    IPlannedSprite this[int index] { get; }
  }
}
