using Surreal.Attributes;
using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

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
    this.amount = amount;
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

  [Template(typeof(AttributeBoostEffect))]
  public sealed record Template : ITemplate<AttributeBoostEffect>
  {
    [Bind] public TimeSpan      Duration  { get; init; } = 30.Seconds();
    [Bind] public AttributeType Attribute { get; init; } = AttributeTypes.Health;
    [Bind] public int           Amount    { get; init; } = 10;

    public AttributeBoostEffect Create() => new(Duration, Attribute, Amount);
  }
}
