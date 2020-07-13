using System.Diagnostics;
using Surreal.Framework.Modifiers;
using Surreal.Mathematics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Int <{Value}>")]
  public class IntParameter : Parameter<int> {
    public IntParameter(int value)
        : base(value) {
    }

    public IntModifierSet Modifiers { get; } = new IntModifierSet();

    public override int Value {
      get => Modifiers.Apply(base.Value);
      set => base.Value = value;
    }
  }

  [DebuggerDisplay("Clamped Int <{Value}> to <{Range}>")]
  public class ClampedIntParameter : IntParameter {
    public ClampedIntParameter(int value, IntRange range)
        : base(value) {
      Range = range;
    }

    public IntRange Range { get; }

    public override int Value {
      get => base.Value;
      set => base.Value = value.Clamp(Range);
    }

    public override void Override(int value) {
      base.Override(value.Clamp(Range));
    }
  }
}