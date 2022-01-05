using Surreal.Mathematics;

namespace Surreal;

/// <summary>A component capable of generating <see cref="TOutput"/>s procedurally.</summary>
public abstract class Generator<TOutput>
{
  public abstract TOutput Generate(Seed seed = default);
}
