using System;
using Surreal;
using Surreal.Timing;

namespace Avventura.Model.Attributes
{
  public class Resource : Attribute
  {
    private EmbeddedTimer regenTimer = new EmbeddedTimer();
    private EmbeddedTimer decayTimer = new EmbeddedTimer();

    public Resource(int value)
      : base(value)
    {
    }

    public event Action<int> Regenerated;
    public event Action<int> Decayed;

    public TimeSpan RegenRate   { get; set; } = 1.Seconds();
    public int      RegenAmount { get; set; } = 0;
    public TimeSpan DecayRate   { get; set; } = 1.Seconds();
    public int      DecayAmount { get; set; } = 0;

    public override void Tick(DeltaTime deltaTime)
    {
      if (RegenAmount > 0 && regenTimer.Tick(deltaTime))
      {
        Value += RegenAmount;
        Regenerated?.Invoke(RegenAmount);

        regenTimer.Reset();
      }

      if (DecayAmount > 0 && decayTimer.Tick(deltaTime))
      {
        Value -= DecayAmount;
        Decayed?.Invoke(DecayAmount);

        decayTimer.Reset();
      }
    }
  }
}
