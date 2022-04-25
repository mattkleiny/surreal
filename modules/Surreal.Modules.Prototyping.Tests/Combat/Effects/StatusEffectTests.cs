using Surreal.Timing;

namespace Surreal.Combat.Effects;

public class StatusEffectTests
{
  [AutoTest]
  public void it_should_invoke_callback_when_adding_effects(object owner, StatusEffect effect)
  {
    var effects = new StatusEffectCollection(owner);

    effects.Add(effect);

    effect.Received(1).OnEffectAdded(owner);
  }

  [AutoTest]
  public void it_should_invoke_callback_when_removing_effects(object owner, StatusEffect effect)
  {
    var effects = new StatusEffectCollection(owner);

    effects.Add(effect);

    effect.Received(1).OnEffectAdded(owner);
  }

  [AutoTest]
  public void it_should_tick_effects_and_remove_if_requested(object owner, StatusEffect effect)
  {
    var effects = new StatusEffectCollection(owner) { effect };

    effects.Update(16.Milliseconds());
    effects.Update(16.Milliseconds());
    effects.Update(16.Milliseconds());
    effects.Update(16.Milliseconds());

    effect.OnEffectUpdate(owner, Arg.Any<TimeDelta>()).Returns(StatusEffect.Transition.Remove);

    effects.Update(16.Milliseconds());

    effect.Received(1).OnEffectRemoved(Arg.Any<object>());

    effects.Should().NotContain(effect);
  }
}
