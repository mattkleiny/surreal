using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Audio;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Sprites;
using Surreal.Input;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal.Framework.Screens {
  public abstract class GameScreen<TGame> : Screen
      where TGame : GameJam {
    protected GameScreen(TGame game)
        : base(game) {
      Assets = new AssetManager(Game.Assets);
    }

    public new TGame Game => (TGame) base.Game;

    public AssetManager     Assets         { get; }
    public IServiceProvider Services       => Game.Services;
    public IAudioDevice     AudioDevice    => Game.AudioDevice;
    public IComputeDevice   ComputeDevice  => Game.ComputeDevice;
    public IGraphicsDevice  GraphicsDevice => Game.GraphicsDevice;
    public IInputManager    InputManager   => Game.InputManager;
    public IKeyboardDevice  Keyboard       => Game.Keyboard;
    public IMouseDevice     Mouse          => Game.Mouse;
    public IScreenManager   Screens        => Game.Screens;
    public SpriteBatch      SpriteBatch    => Game.SpriteBatch;
    public GeometryBatch    GeometryBatch  => Game.GeometryBatch;

    public override void Initialize() {
      LoadContentAsync(Assets).Wait();

      base.Initialize();
    }

    protected virtual Task LoadContentAsync(IAssetResolver assets) {
      return Task.CompletedTask;
    }

    public override void Dispose() {
      Assets.Dispose();

      base.Dispose();
    }
  }
}