using Surreal.Actions;
using Surreal.Utilities;

namespace Isaac.Core.Actions;

[EditorDescription(
  Name = "Add bombs",
  Category = "Inventory",
  Description = "Adds bombs to the owning object's inventory"
)]
public sealed record AddBombs(int Bombs) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Properties.Get(PropertyTypes.Bombs) < 99)
    {
      context.Properties.Increment(PropertyTypes.Bombs, Bombs);
    }

    return ValueTask.CompletedTask;
  }
}
