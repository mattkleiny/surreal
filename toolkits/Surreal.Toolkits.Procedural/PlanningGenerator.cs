using Surreal.Mathematics;

namespace Surreal;

/// <summary>A <see cref="Generator{TOutput}"/> which plans it's output first.</summary>
public abstract class PlanningGenerator<TPlan, TOutput> : Generator<TOutput>
{
  public abstract TPlan   Plan(Seed seed);
  public abstract TOutput Spawn(Seed seed, TPlan plan);

  public sealed override TOutput Generate(Seed seed = default)
  {
    var plan     = Plan(seed);
    var instance = Spawn(seed, plan);

    return instance;
  }
}
