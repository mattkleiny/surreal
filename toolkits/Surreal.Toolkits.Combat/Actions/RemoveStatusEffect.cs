using Surreal.Effects;
using Surreal.Utilities;

namespace Surreal.Actions;

[EditorDescription(
  Name = "Remove status effect",
  Category = "Status Effects",
  Description = "Removes a status effect from the owning object"
)]
public sealed record RemoveStatusEffect(StatusEffect Effect) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Owner is IStatusEffectOwner owner)
    {
      owner.StatusEffects.Remove(Effect);
    }

    return ValueTask.CompletedTask;
  }
}
