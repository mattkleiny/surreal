using System.Diagnostics;
using Surreal.Framework.Modifiers;
using Surreal.Mathematics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Float <{Value}>")]
  public class FloatParameter : Parameter<float> {
    public FloatParameter(float value)
        : base(value) {
    }

    public FloatModifierSet Modifiers { get; } = new();

    public override float Value {
      get => Modifiers.Apply(base.Value);
      set => base.Value = value;
    }
  }

  [DebuggerDisplay("Clamped Float <{Value}> to <{Range}>")]
  public class ClampedFloatParameter : FloatParameter {
    public ClampedFloatParameter(float value, FloatRange range)
        : base(value) {
      Range = range;
    }

    public FloatRange Range { get; }

    public override float Value {
      get => base.Value.Clamp(Range);
      set => base.Value = value.Clamp(Range);
    }

    public override void Override(float value) {
      base.Override(value.Clamp(Range));
    }
  }
}