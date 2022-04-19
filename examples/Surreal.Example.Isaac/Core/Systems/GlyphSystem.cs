using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class GlyphSystem : IteratingSystem
{
  private static Aspect Aspect { get; } = new Aspect()
    .With<Transform>()
    .With<Sprite>();

  private readonly IConsoleDisplay display;

  private IComponentStorage<Transform>? transforms;
  private IComponentStorage<Sprite>? sprites;

  public GlyphSystem(IConsoleDisplay display)
    : base(Aspect)
  {
    this.display = display;
  }

  public override void OnAddedToScene(ActorScene scene)
  {
    base.OnAddedToScene(scene);

    transforms = Scene!.GetStorage<Transform>();
    sprites    = Scene!.GetStorage<Sprite>();
  }

  protected override void OnDraw(DeltaTime deltaTime, ActorId actor)
  {
    base.OnDraw(deltaTime, actor);

    var transform = transforms!.GetComponent(actor);
    var sprite = sprites!.GetComponent(actor);

    display.Draw(16, 16, sprite.Glyph);
  }
}
