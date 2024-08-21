using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// Represents the underlying platform host for the engine.
/// </summary>
public interface IPlatformHost : IDisposable
{
  event Action<DeltaTime> Update;
  event Action<DeltaTime> Render;

  event Action<int, int> Resized;

  int Width { get; }
  int Height { get; }

  bool IsVisible { get; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  /// <summary>
  /// Registers the platform's services in the given registry.
  /// </summary>
  void RegisterServices(IServiceRegistry services);

  /// <summary>
  /// Runs the platform's main loop.
  /// </summary>
  Task RunAsync();

  /// <summary>
  /// Asks the platform to close.
  /// </summary>
  void Close();
}
