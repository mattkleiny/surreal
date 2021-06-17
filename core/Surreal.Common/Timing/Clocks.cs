using System;

namespace Surreal.Timing {
  public interface IClock {
    float     TimeScale { get; set; }
    DeltaTime DeltaTime { get; }
  }

  public static class Clocks {
    public static IClock Relative(IClock other, float scale = 1f) {
      return new AnonymousClock(() => new DeltaTime(other.DeltaTime * scale));
    }

    private sealed class AnonymousClock : IClock {
      private readonly Func<TimeSpan> provider;

      public AnonymousClock(Func<TimeSpan> provider) {
        this.provider = provider;
      }

      public float     TimeScale { get; set; } = 1f;
      public DeltaTime DeltaTime => new(provider() * TimeScale);
    }
  }
}