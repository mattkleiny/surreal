using Avalonia;

namespace Surreal;

/// <summary>An Avalonia <see cref="Application"/> for a <see cref="TGame"/> editor.</summary>
public abstract class EditorApplication<TGame> : Application
  where TGame : Game, new()
{
  public TGame Game { get; } = new();
}
