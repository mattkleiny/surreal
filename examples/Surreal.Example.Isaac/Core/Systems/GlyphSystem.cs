using Isaac.Core.Actors.Components;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering <see cref="Sprite"/>s in the world.</summary>
public sealed class GlyphSystem : IteratingSystem
{
  private readonly IConsoleDisplay display;

  public GlyphSystem(IConsoleDisplay display)
    : base(ComponentMask.Of<Transform, Sprite>())
  {
    this.display = display;
  }
}
