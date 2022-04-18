using Surreal.Mathematics;

namespace Surreal.Procedural;

/// <summary>A tool for performing wave-function-collapse style generation on a space.</summary>
/// <remarks>See https://github.com/mxgmn/WaveFunctionCollapse for more information</remarks>
public sealed class WaveFunctionCollapse<T>
{
  public void Tick()
  {
    throw new NotImplementedException();
  }

  public interface IOutputReceiver
  {
    void SetOutput(Point2 position, T value);
  }

  public sealed record Rule(T Value)
  {
    public List<T> ValidAdjacency { get; } = new();
  }
}
