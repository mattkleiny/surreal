namespace Surreal.Maths;

/// <summary>
/// A seed for the random number generator.
/// </summary>
public readonly record struct Seed(int Value)
{
  public static Seed Default => default;
  public static Seed Randomized => new(Random.Shared.Next());

  /// <summary>
  /// Creates a seed from a string.
  /// </summary>
  public static Seed FromString(string value)
  {
    return new Seed(value.GetHashCode());
  }

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
}
