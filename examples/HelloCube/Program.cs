var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  }
};

Game.Start(configuration, (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard) =>
{
  using var mesh = Mesh.CreateCube(graphics);
  using var material = new Material(graphics, ShaderProgram.LoadDefaultWireShader(graphics));

  var color1 = Random.Shared.Next<Color>();
  var color2 = Random.Shared.Next<Color>();

  var model = Matrix4x4.CreateRotationZ(Angle.FromDegrees(45f), Vector3.Zero);

  var view = Matrix4x4.CreateLookAt(
    cameraPosition: new Vector3(0f, 0f, 20f),
    cameraTarget: new Vector3(0f, 0f, -1f),
    cameraUpVector: Vector3.UnitY
  );

  var projection = Matrix4x4.CreatePerspectiveFieldOfView(
    fieldOfView: Angle.FromDegrees(60f).Radians,
    aspectRatio: 16f / 9f,
    nearPlaneDistance: 0.1f,
    farPlaneDistance: 1000f
  );

  var modelViewProjection = model * view * projection;

  game.ExecuteVariableStep(time =>
  {
    graphics.ClearColorBuffer(Color.Clear);

    var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

    material.Properties.SetProperty(MaterialProperty.Transform,  modelViewProjection);
    material.Properties.SetProperty(MaterialProperty.Color, color);

    mesh.Draw(material);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
