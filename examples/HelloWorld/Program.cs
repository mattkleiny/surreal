using var game = Game.Create(new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1024,
      Height = 768
    }
  }
});

using var world = new EntityWorld(game.Services);

var entity1 = world.SpawnEntity(new Actor { Position = new Vector2(512, 384) });
var entity2 = world.SpawnEntity(new Actor { Position = new Vector2(512, 384) });
var entity3 = world.SpawnEntity(new Actor { Position = new Vector2(512, 384) });

await game.RunAsync(world);

/// <summary>
/// A template for an actor entity.
/// </summary>
internal sealed record Actor : IEntityTemplate
{
  public Vector2 Position { get; set; }
  public Color32 Tint { get; set; } = Color32.White;

  void IEntityTemplate.OnEntitySpawned(EntityWorld world, EntityId entityId)
  {
    world.AddComponent(entityId, new Transform { Position = Position });
  }
}
