using Surreal.Effects;
using Surreal.Objects;
using Surreal.Utilities;

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

  [Template(typeof(FrozenEffect))]
  public sealed record Template : ITemplate<FrozenEffect>
  {
    [Bind] public TimeSpan Duration { get; init; } = 4.Seconds();

    public FrozenEffect Create() => new(Duration);
  }
}
