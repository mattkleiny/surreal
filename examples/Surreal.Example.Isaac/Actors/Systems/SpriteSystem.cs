using Surreal.Aspects;
using Surreal.Graphics.Sprites;
using Surreal.Systems;

namespace Isaac.Actors.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class SpriteSystem : IteratingSystem
{
  private static readonly Aspect TargetAspect = Aspect.Of<Transform, Sprite>();

  private readonly SpriteBatch batch;

  public SpriteSystem(IComponentSystemContext context, SpriteBatch batch)
    : base(context, TargetAspect)
  {
    this.batch = batch;
  }
}
