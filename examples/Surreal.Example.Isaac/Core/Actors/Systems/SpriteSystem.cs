using Isaac.Core.Actors.Components;
using Surreal.Components;
using Surreal.Graphics.Sprites;
using Surreal.Systems;

namespace Isaac.Core.Actors.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class SpriteSystem : IteratingSystem
{
  private readonly SpriteBatch batch;

  public SpriteSystem(IComponentSystemContext context, SpriteBatch batch)
    : base(context, ComponentMask.Of<Transform, Sprite>())
  {
    this.batch = batch;
  }
}
