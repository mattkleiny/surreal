using System;
using Avventura.Model.Attributes;
using Avventura.Model.Dialogues;
using Avventura.Model.Effects;
using Surreal.Framework.Scenes.Actors;
using Surreal.Timing;

namespace Avventura.Model.Actors
{
  public class Character : Actor2D, IDialogueParticipant
  {
    public AttributeSet Attributes { get; } = new AttributeSet();
    public EffectSet    Effects    { get; } = new EffectSet();

    public override void Update(DeltaTime deltaTime)
    {
      base.Update(deltaTime);

      Attributes.Tick(deltaTime);
      Effects.Tick(deltaTime);
    }

    public void Speak(string message)
    {
      throw new NotImplementedException();
    }
  }
}
