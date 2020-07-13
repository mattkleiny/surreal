using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Audio;
using Surreal.Compute;
using Surreal.Diagnostics.Editing;
using Surreal.Graphics;
using Surreal.Graphics.Sprites;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Timing;

namespace Surreal.Framework.Screens {
  public abstract class GameScreen<TGame> : Screen, IEditableScreen
      where TGame : GameJam {
    protected GameScreen(TGame game)
        : base(game) {
      Assets = new AssetManager(Game.Assets);
      Clock  = Clocks.Relative(Game.Clock);
    }

    public new TGame Game => (TGame) base.Game;

    public IServiceProvider Services => Game.Services;

    public IAudioDevice    AudioDevice    => Game.AudioDevice;
    public IComputeDevice  ComputeDevice  => Game.ComputeDevice;
    public IGraphicsDevice GraphicsDevice => Game.GraphicsDevice;
    public IInputManager   InputManager   => Game.InputManager;
    public IKeyboardDevice Keyboard       => Game.Keyboard;
    public IMouseDevice    Mouse          => Game.Mouse;
    public IScreenManager  Screens        => Game.Screens;
    public SpriteBatch     SpriteBatch    => Game.SpriteBatch;

    public AssetManager Assets { get; }

    public override void Initialize() {
      LoadContentAsync(Assets).Wait();

      base.Initialize();
    }

    protected virtual Task LoadContentAsync(IAssetResolver assets) {
      return Task.CompletedTask;
    }

    public virtual void GetEditorProperties(ICollection<EditorProperty> properties) {
      properties.Add(EditorProperty.Anonymous(
          name: nameof(Clock.TimeScale),
          getter: () => Clock.TimeScale,
          setter: value => Clock.TimeScale = value
      ));
    }

    public override void Dispose() {
      Assets.Dispose();

      base.Dispose();
    }
  }
}