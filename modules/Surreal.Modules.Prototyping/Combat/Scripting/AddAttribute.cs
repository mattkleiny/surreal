using Surreal.Combat.Attributes;
using Surreal.Scripting;
using Surreal.Utilities;

namespace Surreal.Combat.Scripting;

[EditorDescription(
  Name = "Add attribute",
  Category = "Attributes",
  Description = "Adds an amount to the owning object's attribute"
)]
public sealed record AddAttribute(AttributeType Attribute, int Amount) : IAction
{
  public ValueTask ExecuteAsync(ActionContext context)
  {
    if (context.Owner is IAttributeOwner owner)
    {
      owner[Attribute] += Amount;
    }

    return ValueTask.CompletedTask;
  }
}
