using Surreal.IO;

namespace Surreal.Mathematics;

/// <summary>
/// A seed for the random number generator.
/// </summary>
public record struct Seed(int Value) : IBinarySerializable<Seed>, IFromRandom<Seed>
{
  public static Seed Default => default;
  public static Seed Randomized => new(Random.Shared.Next());

  /// <summary>
  /// Converts a string to a seed.
  /// </summary>
  public static Seed FromString(string value)
    => new(value.GetHashCode());

  /// <inheritdoc/>
  public static Seed FromRandom(Random random)
    => new(random.NextInt());

  /// <inheritdoc/>
  public static Seed FromBinary(FastBinaryReader reader)
    => new (reader.ReadInt32());

  /// <summary>
  /// Creates a <see cref="Random"/> number generator from the seed.
  /// </summary>
  public Random ToRandom()
  {
    if (Value == 0)
    {
      return Random.Shared;
    }

    return new Random(Value);
  }

  public override string ToString() => $"<{Value}>";

  // conversions
  public static implicit operator Seed(int value) => new(value);

  void IBinarySerializable.ToBinary(FastBinaryWriter writer)
  {
    writer.WriteInt32(Value);
  }
}
