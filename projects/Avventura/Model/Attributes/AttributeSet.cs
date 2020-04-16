using Surreal;
using Surreal.Timing;

namespace Avventura.Model.Attributes
{
  public class AttributeSet
  {
    public Resource Health { get; } = new Resource(10)
    {
      RegenAmount = 0,
      DecayAmount = 0,
    };

    public Resource Toxin { get; } = new Resource(0)
    {
      DecayAmount = 1,
      DecayRate   = 500.Milliseconds()
    };

    public Resource Stamina { get; } = new Resource(100)
    {
      RegenAmount = 1,
      RegenRate   = 500.Milliseconds()
    };

    public virtual void Tick(DeltaTime deltaTime)
    {
      Health.Tick(deltaTime);
      Toxin.Tick(deltaTime);
      Stamina.Tick(deltaTime);
    }
  }
}
