using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// Represents the underlying platform.
/// </summary>
public interface IPlatform
{
  /// <summary>
  /// Builds the main host for the platform.
  /// </summary>
  IPlatformHost BuildHost();
}

/// <summary>
/// Represents the underlying platform host for the engine.
/// </summary>
public interface IPlatformHost : IDisposable
{
  event Action<int, int> Resized;

  int Width { get; }
  int Height { get; }
  bool IsVisible { get; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  void BeginFrame(DeltaTime deltaTime);
  void EndFrame(DeltaTime deltaTime);

  /// <summary>
  /// Registers the platform's services in the given registry.
  /// </summary>
  void RegisterServices(IServiceRegistry services);
}
