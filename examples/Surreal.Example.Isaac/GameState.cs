using System;
using System.IO;
using System.Numerics;
using Isaac.Core.Items;
using Surreal.Framework.Parameters;
using Surreal.IO;
using Surreal.Mathematics;
using Range = Surreal.Mathematics.Range;

namespace Isaac {
  public sealed class GameState {
    private const int CurrentVersion = 1;

    public int  Version { get; private set; } = CurrentVersion;
    public Seed Seed    { get; set; }         = Seed.Randomized;

    public PlayerState Player { get; } = new PlayerState();

    public void Save(BinaryWriter writer) {
      writer.Write(Version);
      writer.Write(Seed);

      Player.Save(writer);
    }

    public void Load(BinaryReader reader) {
      var version = reader.ReadInt32();
      if (version > CurrentVersion) {
        throw new Exception("The save appears to be made with a more recent version of the game!");
      }

      Version = version;
      Seed    = reader.ReadSeed();

      Player.Load(reader);
    }
  }

  public sealed class PlayerState {
    private static readonly IntRange   HealthRange = Range.Of(0, 100);
    private static readonly IntRange   CoinsRange  = Range.Of(0, 99);
    private static readonly FloatRange SpeedRange  = Range.Of(0f, 10f);

    public Vector2Parameter      Position  { get; } = new Vector2Parameter(Vector2.Zero);
    public AngleParameter        Rotation  { get; } = new AngleParameter(Angle.Zero);
    public ClampedFloatParameter Speed     { get; } = new ClampedFloatParameter(4f, SpeedRange);
    public ClampedIntParameter   Health    { get; } = new ClampedIntParameter(4, HealthRange);
    public ClampedIntParameter   Coins     { get; } = new ClampedIntParameter(0, CoinsRange);
    public Inventory             Inventory { get; } = new Inventory();

    public void Save(BinaryWriter writer) {
      writer.Write(Health);
      writer.Write(Coins);
      writer.Write(Position);

      Inventory.Save(writer);
    }

    public void Load(BinaryReader reader) {
      Health.Value   = reader.ReadInt32();
      Coins.Value    = reader.ReadInt32();
      Position.Value = reader.ReadVector2();

      Inventory.Load(reader);
    }
  }
}