using Surreal.Mathematics;

namespace Surreal.Procedural;

/// <summary>A tool for performing wave-function-collapse style generation on a space.</summary>
/// <remarks>See https://github.com/mxgmn/WaveFunctionCollapse for more information</remarks>
public sealed class WaveFunctionCollapse<T>
{
  public List<Rule> Rules { get; } = new();

  public void Seed(Point2 position, T value)
  {
    throw new NotImplementedException();
  }

  public void Tick(int maxSteps = int.MaxValue)
  {
    throw new NotImplementedException();
  }

  public sealed record Rule(T Value)
  {
    public List<T> ValidAdjacency { get; } = new();
  }
}
