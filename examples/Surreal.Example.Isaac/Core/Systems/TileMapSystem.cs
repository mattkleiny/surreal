using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Components;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering tile maps in the world.</summary>
public sealed class TileMapSystem : IteratingSystem
{
  private readonly IConsoleDisplay display;

  public TileMapSystem(IConsoleDisplay display)
    : base(Aspect.Of<Transform, Sprite>())
  {
    this.display = display;
  }
}
