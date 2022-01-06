using Surreal.Components;
using Surreal.Graphics.Sprites;
using Surreal.Systems;

namespace Isaac.Actors.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class SpriteSystem : IteratingSystem
{
  private static readonly ComponentMask TargetMask = ComponentMask.Of<Transform, Sprite>();

  private readonly SpriteBatch batch;

  public SpriteSystem(IComponentSystemContext context, SpriteBatch batch)
    : base(context, TargetMask)
  {
    this.batch = batch;
  }
}
