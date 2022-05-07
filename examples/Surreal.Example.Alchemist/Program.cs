var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Alchemist",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
    IconPath       = "resx://Alchemist/Resources/icons/alchemist.png"
  }
};

Game.Start(platform, async game =>
{
  // ReSharper disable AccessToDisposedClosure
  var graphics = game.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = game.Services.GetRequiredService<IKeyboardDevice>();

  using var mesh = new Mesh<Vertex2>(graphics, BufferUsage.Dynamic);
  using var texture = Texture.CreateColored(graphics, Color.White);
  using var shader = await game.Assets.LoadDefaultSpriteShaderAsync();

  void RebuildMesh()
  {
    var color1 = Random.Shared.NextColor();
    var color2 = Random.Shared.NextColor();

    mesh.Tessellate(tessellator =>
    {
      const int numberOfPoints = 16;
      const float innerRadius = 0.25f;
      const float outerRadius = 1f;

      var random = Random.Shared;
      var points = new SpanList<Vertex2>(stackalloc Vertex2[numberOfPoints]);

      var theta = 0f;

      for (var i = 0; i < numberOfPoints; i++)
      {
        theta += 2 * MathF.PI / numberOfPoints;

        var radius = random.NextFloat(innerRadius, outerRadius);

        var x = radius * MathF.Cos(theta);
        var y = radius * MathF.Sin(theta);
        var color = Color.Lerp(color1, color2, i / (float) numberOfPoints);

        points.Add(new Vertex2(new(x, y), color, new Vector2(0f, 0f)));
      }

      tessellator.AddTriangleFan(points);
    });
  }

  RebuildMesh();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    if (keyboard.IsKeyPressed(Key.Space))
    {
      RebuildMesh();
    }

    graphics.ClearColorBuffer(Color.Black);

    shader.SetUniform("u_projectionView", Matrix4x4.Identity);
    shader.SetUniform("u_texture", texture, 0);

    mesh.Draw(shader);
  });
});
