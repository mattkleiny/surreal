using Surreal.Aspects;
using Surreal.Systems;

namespace Isaac.Actors.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class SpriteSystem : IteratingSystem
{
  private static Aspect TargetAspect { get; } = Aspect.Of<Transform, Sprite>();

  public SpriteSystem(IComponentSystemContext context, Game game)
    : base(context, TargetAspect)
  {
  }
}
