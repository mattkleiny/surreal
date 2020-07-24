using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surreal.Mathematics.Timing {
  public interface IClock {
    float     TimeScale { get; set; }
    DeltaTime DeltaTime { get; }
  }

  public sealed class FixedStepClock : IClock {
    public static FixedStepClock CreateDefault() => new FixedStepClock(16.Milliseconds());

    public FixedStepClock(DeltaTime deltaTime) {
      DeltaTime = deltaTime;
    }

    public float     TimeScale { get; set; } = 1f;
    public DeltaTime DeltaTime { get; private set; }

    public void Tick(DeltaTime time) {
      DeltaTime = new DeltaTime(time * TimeScale);
    }
  }

  public static class Clocks {
    public static IClock Relative(IClock other, float scale = 1f) {
      return new AnonymousClock(() => new DeltaTime(other.DeltaTime * scale));
    }

    public static async Task EvaluateOverTime(this IClock clock, TimeSpan duration, Action<Quanta> action, CancellationToken cancellationToken = default) {
      var currentTime  = 0f;
      var totalSeconds = (float) duration.TotalSeconds;

      while (currentTime <= totalSeconds && !cancellationToken.IsCancellationRequested) {
        currentTime += clock.DeltaTime;
        action(new Quanta(currentTime, totalSeconds));

        await Task.Yield();
      }
    }

    private sealed class AnonymousClock : IClock {
      private readonly Func<TimeSpan> provider;

      public AnonymousClock(Func<TimeSpan> provider) {
        this.provider = provider;
      }

      public float     TimeScale { get; set; } = 1f;
      public DeltaTime DeltaTime => new DeltaTime(provider() * TimeScale);
    }
  }
}