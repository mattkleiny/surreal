namespace Surreal.Mathematics;

/// <summary>A seed for the random number generator.</summary>
public readonly record struct Seed(int Value)
{
  public static Seed Default    => default;
  public static Seed Randomized => new(Random.Shared.Next());

  public Random ToRandom()
  {
    if (Value == 0)
    {
      return Random.Shared;
    }

    return new Random(Value);
  }

  public override string ToString() => $"<{Value.ToString()}>";
}