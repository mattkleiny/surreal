namespace Surreal.Timing
{
  public sealed class FrameTimer
  {
    private readonly int frameCount;
    private          int accumulator;

    public FrameTimer(int frameCount)
    {
      Check.That(frameCount > 0, "frameCount > 0");

      this.frameCount = frameCount;
    }

    public bool Tick()
    {
      if (accumulator++ >= frameCount)
      {
        accumulator = 0;
        return true;
      }

      return false;
    }
  }
}
