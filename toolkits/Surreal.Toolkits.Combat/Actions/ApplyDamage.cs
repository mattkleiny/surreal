using Surreal.Utilities;

namespace Surreal.Actions;

[EditorDescription(
  Name = "Apply damage",
  Category = "Combat",
  Description = "Damages the associating object"
)]
public sealed record ApplyDamage(Damage Damage) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Owner is IDamageReceiver receiver)
    {
      receiver.OnDamageReceived(Damage);
    }

    return ValueTask.CompletedTask;
  }
}
