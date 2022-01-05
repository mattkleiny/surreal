using NUnit.Framework;
using Surreal.Effects;

namespace Surreal;

public class StatusEffectTests
{
  [Test, AutoFixture]
  public void it_should_invoke_callback_when_adding_effects(object owner, StatusEffect effect)
  {
    var effects = new StatusEffectCollection(owner);

    effects.Add(effect);

    effect.Received(Quantity.Exactly(1)).OnEffectAdded(owner);
  }

  [Test, AutoFixture]
  public void it_should_invoke_callback_when_removing_effects(object owner, StatusEffect effect)
  {
    var effects = new StatusEffectCollection(owner);

    effects.Add(effect);

    effect.Received(Quantity.Exactly(1)).OnEffectAdded(owner);
  }
}
