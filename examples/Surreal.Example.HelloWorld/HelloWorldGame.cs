using System.Runtime.InteropServices;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Input.Keyboard;

var platform = new DesktopPlatform
{
  Configuration =
  {
    Title          = "Hello, Surreal!",
    IsVsyncEnabled = true,
    ShowFpsInTitle = true,
  },
};

await Game.Start(platform, context =>
{
  var projection = Matrix4x4.CreateOrthographic(256f, 144f, 0f, 100f);

  var graphics = context.Services.GetRequiredService<IGraphicsServer>();
  var keyboard = context.Services.GetRequiredService<IKeyboardDevice>();

  using var shader = context.Assets.LoadAsset<ShaderProgram>("Assets/shaders/helloworld.glsl");
  using var batch = new GeometryBatch(graphics);

  var position = Vector2.Zero;

  context.Execute(time =>
  {
    const float speed = 100f;

    if (keyboard.IsKeyPressed(Key.Escape)) context.Exit();

    if (keyboard.IsKeyDown(Key.W)) position.Y += 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.S)) position.Y -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.A)) position.X -= 1 * speed * time.DeltaTime;
    if (keyboard.IsKeyDown(Key.D)) position.X += 1 * speed * time.DeltaTime;

    graphics.ClearColorBuffer(Color.Black);

    if (shader.IsReady)
    {
      shader.Value.SetUniform("u_projectionView", in projection);

      batch.Begin(shader);

      batch.DrawSolidQuad(position, new Vector2(20f, 20f), Color.Yellow);
    }
  });

  return ValueTask.CompletedTask;
});
