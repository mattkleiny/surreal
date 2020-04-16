using System;
using Surreal.Framework.Parameters;
using Surreal.Timing;

namespace Avventura.Model.Attributes
{
  public class Attribute : ClampedIntParameter
  {
    protected Attribute(int value = default)
      : base(value, Surreal.Mathematics.Range.Of(0, 100))
    {
    }

    public event Action<int> Changed;

    public override int Value
    {
      get => base.Value;
      set
      {
        base.Value = value;
        Changed?.Invoke(value);
      }
    }

    public virtual void Tick(DeltaTime time)
    {
    }
  }
}
