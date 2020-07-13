using System;

namespace Surreal.Framework.Modifiers {
  public sealed class IntModifierSet : ModifierSet<int> {
    protected override int Calculate(int baseValue, ReadOnlySpan<Modifier<int>> modifiers) {
      var value      = baseValue;
      var cumulative = 0;

      for (var i = 0; i < modifiers.Length; i++) {
        var modifier = modifiers[i];

        switch (modifier.Type) {
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

      if (cumulative > 0) {
        value *= cumulative;
      }

      return value;
    }
  }
}