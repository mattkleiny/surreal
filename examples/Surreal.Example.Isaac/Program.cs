using Isaac.Dungeons;
using Surreal.Scripting;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "The Binding of Isaac",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure

  // grab services
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var input = game.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // set-up scripting
  game.Assets.AddLoader(new ScriptLoader(new LuaScriptServer(), ".lua"));

  var script = await game.Assets.LoadAssetAsync<Script>("Assets/scripts/player.lua");

  // load assets
  using var batch = new GeometryBatch(graphics);
  using var shader = await game.Assets.LoadDefaultShaderAsync();
  using var texture = Texture.CreateColored(graphics, Color.White);
  using var scene = new ActorScene();

  // set-up a basic camera
  var camera = new Camera
  {
    Position = new(-256f / 2f, -144f / 2f),
    Size     = new Vector2(256, 144),
  };

  // plan a random dungeon
  var random = Random.Shared;
  var plan = new RoomPlanGrid();

  plan.Add(new RoomPlan
  {
    Position = new Point2(256 / 2, 144 / 2),
    Type     = RoomType.Spawn
  });

  game.ExecuteVariableStep(time =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      var last = plan.Last!;
      var direction = random.NextEnumMask(Direction.All);

      plan.Add(last.AddChild(direction));
    }

    shader.SetUniform("u_projectionView", in camera.ProjectionView);
    shader.SetUniform("u_texture", texture, 0);

    script.ExecuteFunction("update");

    scene.BeginFrame(time.DeltaTime);
    scene.Input(time.DeltaTime);
    scene.Update(time.DeltaTime);
    scene.Draw(time.DeltaTime);
    scene.EndFrame(time.DeltaTime);

    batch.Begin(shader);
    plan.First!.DrawGizmos(batch);
  });
});
