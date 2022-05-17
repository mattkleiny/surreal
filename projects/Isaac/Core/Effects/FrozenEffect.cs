using Surreal.Systems.Effects;

namespace Isaac.Core.Effects;

[EditorDescription(
  Name = "Frozen",
  Category = "Status Effects",
  Description = "Freezes an object in-place, preventing it's movement"
)]
public sealed class FrozenEffect : PermanentStatusEffect
{
  public FrozenEffect(TimeSpan duration)
    : base(duration)
  {
  }

  public override StatusEffectType Type => StatusEffectTypes.Frozen;
}
