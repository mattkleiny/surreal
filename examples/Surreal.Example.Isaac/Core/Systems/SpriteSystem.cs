using Isaac.Core.Actors.Components;
using Surreal.Components;
using Surreal.Graphics.Meshes;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class SpriteSystem : IteratingSystem
{
  private readonly SpriteBatch batch;

  public SpriteSystem(SpriteBatch batch)
    : base(ComponentMask.Of<Transform, Sprite>())
  {
    this.batch = batch;
  }
}
