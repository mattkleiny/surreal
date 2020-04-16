using Surreal.Timing;

namespace Avventura.Model.Effects
{
  public abstract class Effect
  {
    public abstract EffectStatus Tick(DeltaTime deltaTime);
  }
}
