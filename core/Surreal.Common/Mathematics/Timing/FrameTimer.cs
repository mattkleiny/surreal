using System.Diagnostics;

namespace Surreal.Mathematics.Timing {
  public struct FrameTimer {
    private readonly int frameCount;
    private          int accumulator;

    public FrameTimer(int frameCount) {
      Debug.Assert(frameCount > 0, "frameCount > 0");

      this.frameCount = frameCount;
      accumulator     = 0;
    }

    public bool Tick() {
      if (accumulator++ >= frameCount) {
        accumulator = 0;
        return true;
      }

      return false;
    }
  }
}