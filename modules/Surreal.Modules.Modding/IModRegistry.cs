namespace Surreal;

/// <summary>The registry interface for interacting with <see cref="IMod"/>s.</summary>
public interface IModRegistry
{
  IServiceProvider Services { get; }
}
