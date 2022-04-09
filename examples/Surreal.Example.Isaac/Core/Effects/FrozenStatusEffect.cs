using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

namespace Isaac.Core.Effects;

[EditorDescription(
  Name = "Frozen",
  Category = "Status Effects",
  Description = "Freezes an object in-place, preventing it's movement"
)]
public sealed class FrozenStatusEffect : PermanentStatusEffect
{
  public FrozenStatusEffect(TimeSpan duration)
    : base(duration)
  {
  }

  public override StatusEffectType Type => StatusEffectTypes.Frozen;

  [Template(typeof(FrozenStatusEffect))]
  public sealed record Template : ITemplate<FrozenStatusEffect>
  {
    public TimeSpan Duration { get; init; } = 4.Seconds();

    public FrozenStatusEffect Create()
    {
      return new FrozenStatusEffect(Duration);
    }
  }
}
