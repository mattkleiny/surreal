using Surreal.Combat.Effects;
using Surreal.Objects;
using Surreal.Scripting;
using Surreal.Utilities;

namespace Surreal.Combat.Scripting;

[EditorDescription(
  Name = "Add status effect",
  Category = "Status Effects",
  Description = "Adds a status effect to the owning object"
)]
public sealed record AddStatusEffect(ITemplate<StatusEffect> Effect) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Owner is IStatusEffectOwner owner)
    {
      owner.StatusEffects.Add(Effect.Create());
    }

    return ValueTask.CompletedTask;
  }
}
