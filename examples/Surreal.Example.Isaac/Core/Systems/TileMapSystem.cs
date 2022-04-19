using Isaac.Core.Actors.Components;
using Surreal.Aspects;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering tile maps in the world.</summary>
public sealed class TileMapSystem : IteratingSystem
{
  private static Aspect Aspect { get; } = new Aspect()
    .With<Transform>()
    .With<Sprite>();

  private readonly IConsoleDisplay display;

  public TileMapSystem(IConsoleDisplay display)
    : base(Aspect)
  {
    this.display = display;
  }
}
