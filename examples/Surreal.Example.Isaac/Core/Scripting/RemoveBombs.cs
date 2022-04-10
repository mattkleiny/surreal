using Surreal.Scripting;
using Surreal.Utilities;

namespace Isaac.Core.Scripting;

[EditorDescription(
  Name = "Remove bombs",
  Category = "Inventory",
  Description = "Removes bombs from the owning object's inventory"
)]
public sealed record RemoveBombs(int Bombs) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(PropertyTypes.Bombs) > 0)
    {
      context.Properties.Decrement(PropertyTypes.Bombs, Bombs);
    }

    return ValueTask.CompletedTask;
  }
}
