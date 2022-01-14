using Surreal.Assets;
using Surreal.Controls;
using Surreal.Graphics.Materials;
using Surreal.Input.Keyboard;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  private Material spriteMaterial;

  public static Task Main() => GameEditor.StartAsync<IsaacGame>(new Configuration
  {
    Platform = new DesktopPlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        IsVsyncEnabled = true,
        ShowFpsInTitle = true,
      },
    },
  });

  protected override async Task LoadContentAsync(IAssetManager assets, CancellationToken cancellationToken = default)
  {
    await base.LoadContentAsync(assets, cancellationToken);

    spriteMaterial = await assets.LoadAssetAsync<Material>("Assets/shaders/sprite.shader");
  }

  protected override void Input(GameTime time)
  {
    if (Keyboard.IsKeyPressed(Key.Escape)) Exit();
    if (Keyboard.IsKeyPressed(Key.F8)) GameEditor.ShowWindow(new GraphEditorWindow());
    if (Keyboard.IsKeyPressed(Key.F9)) GameEditor.ShowWindow(new TileGridEditorWindow());

    base.Input(time);
  }
}
