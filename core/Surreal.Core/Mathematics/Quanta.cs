using System.Runtime.CompilerServices;

namespace Surreal.Mathematics {
  public readonly struct Quanta {
    public Quanta(float currentTime, float totalTime) {
      CurrentTime = currentTime;
      TotalTime   = totalTime;
    }

    public float CurrentTime { get; }
    public float TotalTime   { get; }

    public Normal Normal {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new(CurrentTime / TotalTime);
    }
  }
}