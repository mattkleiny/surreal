using Alchemist.Graphics;

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

  using var mesh = IslandMeshes.Create(graphics);
  using var texture = Texture.CreateColored(graphics, Color.White);
  using var shader = await game.Assets.LoadDefaultSpriteShaderAsync();

  game.ExecuteVariableStep(_ =>
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }

    graphics.ClearColorBuffer(Color.Black);

    shader.SetUniform("u_projectionView", Matrix4x4.Identity);
    shader.SetUniform("u_texture", texture, 0);

    mesh.Draw(shader);
  });
});
