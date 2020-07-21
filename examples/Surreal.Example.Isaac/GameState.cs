using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Isaac.Core.Items;
using Surreal.Diagnostics.Logging;
using Surreal.Framework.Parameters;
using Surreal.IO;
using Surreal.Mathematics;
using Path = Surreal.IO.Path;
using Range = Surreal.Mathematics.Range;

namespace Isaac {
  public sealed class GameState {
    private static readonly ILog Log = LogFactory.GetLog<GameState>();

    private const int CurrentVersion = 1;

    public int         Version { get; private set; } = CurrentVersion;
    public Seed        Seed    { get; private set; } = Seed.Randomized;
    public PlayerState Player  { get; private set; } = new PlayerState();

    #region Persistence

    public static async Task<GameState> LoadAsync(Path path) {
      Log.Trace($"Loading game from {path}");

      await using var stream = await path.OpenInputStreamAsync();
      using var       reader = new BinaryReader(stream, Encoding.UTF8);

      return Load(reader);
    }

    public async Task SaveAsync(Path path) {
      Log.Trace($"Saving game to {path}");

      await using var stream = await path.OpenOutputStreamAsync();
      await using var writer = new BinaryWriter(stream, Encoding.UTF8);

      Save(writer);
    }

    public static GameState Load(BinaryReader reader) {
      var version = reader.ReadInt32();
      if (version > CurrentVersion) {
        throw new Exception("The save appears to be made with a more recent version of the game!");
      }

      return new GameState {
          Version = version,
          Seed    = reader.ReadSeed(),
          Player  = PlayerState.Load(reader)
      };
    }

    public void Save(BinaryWriter writer) {
      writer.Write(Version);
      writer.Write(Seed);

      Player.Save(writer);
    }

    #endregion
  }

  public sealed class PlayerState {
    private static readonly IntRange HealthRange = Range.Of(0, 100);
    private static readonly IntRange CoinsRange  = Range.Of(0, 99);

    public Parameter<int>     Health    { get; private set; } = new ClampedIntParameter(4, HealthRange);
    public Parameter<int>     Coins     { get; private set; } = new ClampedIntParameter(0, CoinsRange);
    public Parameter<Vector2> Position  { get; private set; } = new Vector2Parameter(Vector2.Zero);
    public Inventory          Inventory { get; private set; } = new Inventory();

    #region Persistence

    public void Save(BinaryWriter writer) {
      writer.Write(Health.Value);
      writer.Write(Coins.Value);
      writer.Write(Position.Value);
    }

    public static PlayerState Load(BinaryReader reader) {
      return new PlayerState {
          Health   = new ClampedIntParameter(reader.ReadInt32(), HealthRange),
          Coins    = new ClampedIntParameter(reader.ReadInt32(), CoinsRange),
          Position = new Vector2Parameter(reader.ReadVector2()),
      };
    }

    #endregion
  }
}