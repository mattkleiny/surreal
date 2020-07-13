using System;
using Avventura.Model.Actors;
using Surreal;
using Surreal.Timing;

namespace Avventura.Model.Effects {
  public abstract class TimedEffect : Effect {
    private EmbeddedTimer tickTimer     = new EmbeddedTimer();
    private EmbeddedTimer durationTimer = new EmbeddedTimer();

    public TimeSpan Duration  { get; set; } = 10.Seconds();
    public TimeSpan Frequency { get; set; } = 1.Seconds();

    public sealed override EffectStatus Tick(DeltaTime deltaTime, Character character) {
      if (tickTimer.Tick(deltaTime)) {
        NextTick(character);
        tickTimer.Reset();
      }

      if (durationTimer.Tick(deltaTime)) {
        return EffectStatus.Remove;
      }

      return EffectStatus.Continue;
    }

    protected abstract void NextTick(Character character);
  }
}