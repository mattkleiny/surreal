using Surreal.Attributes;
using Surreal.Utilities;

namespace Surreal.Actions;

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
