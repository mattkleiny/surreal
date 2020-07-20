using System.Numerics;
using Surreal.Framework.Parameters;
using Surreal.Mathematics;

namespace Isaac {
  public sealed class GameState {
    public Seed        Seed   { get; } = Seed.Randomized;
    public PlayerState Player { get; } = new PlayerState();

    public sealed class PlayerState {
      public ClampedIntParameter Health   { get; } = new ClampedIntParameter(4, Range.Of(0, 100));
      public ClampedIntParameter Coins    { get; } = new ClampedIntParameter(0, Range.Of(0, 99));
      public Vector2Parameter    Position { get; } = new Vector2Parameter(Vector2.Zero);
    }
  }
}