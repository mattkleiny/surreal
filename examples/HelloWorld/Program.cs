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

var color1 = Random.Shared.NextColor();
var color2 = Random.Shared.NextColor();
var totalTime = 0.0f;

var entity = world.SpawnEntity();

world.AddComponent(entity, new Transform());
world.AddComponent(entity, new Sprite());

world.AddSystem<VariableTick>((IKeyboardDevice keyboard) =>
{
  if (keyboard.IsKeyPressed(Key.Escape))
  {
    game.Exit();
  }

  if (keyboard.IsKeyPressed(Key.Space))
  {
    color1 = Random.Shared.NextColor();
    color2 = Random.Shared.NextColor();
  }
});

world.AddSystem<RenderFrame>((RenderFrame @event) =>
{
  totalTime += @event.DeltaTime;

  @event.Device.ClearColorBuffer(Color.Lerp(color1, color2, MathE.PingPong(totalTime)));
});

await game.RunAsync(world);
