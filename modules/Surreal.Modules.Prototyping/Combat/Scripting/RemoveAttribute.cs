using Surreal.Combat.Attributes;
using Surreal.Scripting;
using Surreal.Utilities;

namespace Surreal.Combat.Scripting;

[EditorDescription(
  Name = "Remove attribute",
  Category = "Attributes",
  Description = "Removes an amount to the owning object's attribute"
)]
public sealed record RemoveAttribute(AttributeType Attribute, int Amount) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Owner is IAttributeOwner owner)
    {
      owner[Attribute] -= Amount;
    }

    return ValueTask.CompletedTask;
  }
}
