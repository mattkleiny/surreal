using Surreal.Systems.Attributes;
using Surreal.Systems.Effects;

namespace Isaac.Core.Effects;

[EditorDescription(
  Name = "Health Boost",
  Category = "Status Effects",
  Description = "Temporarily boosts an attribute of an object"
)]
public sealed class AttributeBoostEffect : PermanentStatusEffect
{
  private readonly AttributeType attribute;
  private readonly int amount;

  public AttributeBoostEffect(TimeSpan duration, AttributeType attribute, int amount)
    : base(duration)
  {
    this.attribute = attribute;
    this.amount    = amount;
  }

  public override void OnEffectAdded(object target)
  {
    base.OnEffectAdded(target);

    if (target is IAttributeOwner owner)
    {
      owner[attribute] += amount;
    }
  }

  public override void OnEffectRemoved(object target)
  {
    base.OnEffectRemoved(target);

    if (target is IAttributeOwner owner)
    {
      owner[attribute] -= amount;
    }
  }
}
