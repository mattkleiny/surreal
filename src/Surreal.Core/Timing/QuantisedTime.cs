using Surreal.Mathematics;

namespace Surreal.Timing
{
  public readonly struct QuantisedTime
  {
    public QuantisedTime(float currentTime, float totalTime)
    {
      CurrentTime = currentTime;
      TotalTime   = totalTime;
    }

    public float CurrentTime { get; }
    public float TotalTime   { get; }

    public Normal NormalizedTime => new Normal(CurrentTime / TotalTime);
  }
}
