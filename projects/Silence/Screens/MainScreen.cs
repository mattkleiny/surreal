using System.Numerics;
using System.Threading.Tasks;
using Silence.Core.Simulation;
using Surreal;
using Surreal.Assets;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;
using Surreal.Graphics;
using Surreal.Input.Keyboard;

namespace Silence.Screens {
  public sealed class MainScreen : SimulationScreen<SilenceGame, ActorSimulation>, ILoadableScreen {
    private const float Speed = 20f;

    private Vector2 position;

    public MainScreen(SilenceGame game)
        : base(game) {
    }

    protected override ActorSimulation CreateSimulation() {
      return new GameSimulation(Game);
    }

    Task ILoadableScreen.LoadInBackgroundAsync(IAssetResolver assets) {
      return Task.CompletedTask;
    }

    public override void Input(GameTime time) {
      base.Input(time);

      var direction = Vector2.Zero;

      if (Keyboard.IsKeyDown(Key.W)) direction.Y += 1;
      if (Keyboard.IsKeyDown(Key.S)) direction.Y -= 1;
      if (Keyboard.IsKeyDown(Key.A)) direction.X -= 1;
      if (Keyboard.IsKeyDown(Key.D)) direction.X += 1;

      position += direction * Speed * time.DeltaTime;
    }

    public override void Draw(GameTime time) {
      base.Draw(time);

      Game.GeometryBatch.DrawCircle(position, 3f, Color.Red, 16);
    }
  }
}