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
  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var input = context.Services.GetRequiredService<IInputServer>();
  var keyboard = input.GetRequiredDevice<IKeyboardDevice>();

  using var batch = new GeometryBatch(graphics);
  using var shader = await context.Assets.LoadDefaultShaderAsync();
  using var texture = Texture.CreateColored(graphics, Color.White);

  // set-up a basic camera perspective
  var projectionView =
    Matrix4x4.CreateTranslation(-256f / 2f, -144f / 2f, 0f) * // view
    Matrix4x4.CreateOrthographic(256f, 144f, 0f, 100f);       // projection

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

    shader.SetUniform("u_projectionView", in projectionView);
    shader.SetUniform("u_texture", texture, 0);

    batch.Begin(shader);
    plan.First!.DrawGizmos(batch);
  });
});
