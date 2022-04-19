using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class GlyphSystem : IteratingSystem
{
  private readonly IConsoleDisplay display;

  public GlyphSystem(IConsoleDisplay display)
    : base(Aspect.Of<Transform, Sprite>())
  {
    this.display = display;
  }

  protected override void OnDraw(DeltaTime deltaTime, ActorId actor)
  {
    base.OnDraw(deltaTime, actor);

    var transform = GetComponent<Transform>(actor);
    var sprite = GetComponent<Sprite>(actor);

    display.Draw(16, 16, sprite.Glyph);
  }
}
