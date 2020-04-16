using System;

namespace Surreal.Framework.Modifiers
{
  public sealed class FloatModifierSet : ModifierSet<float>
  {
    protected override float Calculate(float baseValue, ReadOnlySpan<Modifier<float>> modifiers)
    {
      var value      = baseValue;
      var cumulative = 0f;

      for (var i = 0; i < modifiers.Length; i++)
      {
        var modifier = modifiers[i];

        switch (modifier.Type)
        {
          case ModifierType.Additive:
            value += modifier.Amount;
            break;

          case ModifierType.Cumulative:
            cumulative += modifier.Amount;
            break;

          case ModifierType.Multiplicative:
            value *= modifier.Amount;
            break;
        }
      }

      if (cumulative > 0f)
      {
        value *= cumulative;
      }

      return value;
    }
  }
}