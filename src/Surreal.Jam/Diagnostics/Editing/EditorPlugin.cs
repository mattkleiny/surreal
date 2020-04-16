using System.Collections.Generic;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Diagnostics.Editing.Controls;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Input.Keyboard;

namespace Surreal.Diagnostics.Editing
{
  public sealed class EditorPlugin : DiagnosticPlugin<GameJam>
  {
    private readonly List<EditorProperty> properties = new List<EditorProperty>();

    public EditorPlugin(GameJam game)
      : base(game)
    {
    }

    public IKeyboardDevice Keyboard => Game.Keyboard;
    public IScreenManager  Screens  => Game.Screens;

    public override void Initialize()
    {
      base.Initialize();

      Screens.ScreenChanged += OnScreenChanged;
    }

    public override async Task LoadContentAsync(IAssetResolver assets)
    {
      await base.LoadContentAsync(assets);

      var font = await assets.LoadDefaultFontAsync();

      Stage.Add(new EditorPanel(font));
    }

    public override void Begin()
    {
      base.Begin();

      Game.GeometryBatch.Begin(Stage.Viewport());
    }

    public override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.F2)) IsVisible = !IsVisible;

      base.Input(time);
    }

    public override void End()
    {
      Game.GeometryBatch.End();

      base.End();
    }

    public override void Dispose()
    {
      Screens.ScreenChanged -= OnScreenChanged;

      base.Dispose();
    }

    private void OnScreenChanged(IScreen screen)
    {
      properties.Clear();

      if (screen is IEditableScreen editable)
      {
        editable.GetEditorProperties(properties);
      }
    }
  }
}