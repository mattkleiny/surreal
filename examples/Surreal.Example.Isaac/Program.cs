using Isaac.Dungeons;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "The Binding of Isaac",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true
  }
};

Game.Start(platform, async context =>
{
  // grab services
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  // load assets
  using var batch = new GeometryBatch(graphics);
  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var texture = Texture.CreateColored(graphics, Color.White);

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

  context.ExecuteVariableStep(_ =>
  {
    if (!context.Host.IsFocused)
    {
      return;
    }

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      context.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      var last = plan.Last!;
      var direction = random.NextEnumMask(Direction.All);

      plan.Add(last.AddChild(direction));
    }

    shader.SetUniform("u_projectionView", in camera.ProjectionView);
    shader.SetUniform("u_texture", texture, 0);

    batch.Begin(shader);
    plan.First!.DrawGizmos(batch);
  });
});
