namespace Surreal.Timing;

/// <summary>Represents a clock that can measure time elapsed.</summary>
public interface IClock
{
  DeltaTime DeltaTime { get; }
}

public static class Clocks
{
  /// <summary>Builds a <see cref="IClock"/> that is relative to some other clock with a fixed <see cref="scale"/>.</summary>
  public static IClock Relative(IClock other, float scale = 1f)
  {
    return new AnonymousClock(() => new DeltaTime(other.DeltaTime * scale));
  }

  private sealed class AnonymousClock : IClock
  {
    private readonly Func<TimeSpan> provider;

    public AnonymousClock(Func<TimeSpan> provider)
    {
      this.provider = provider;
    }

    public float     TimeScale { get; set; } = 1f;
    public DeltaTime DeltaTime => new(provider() * TimeScale);
  }
}