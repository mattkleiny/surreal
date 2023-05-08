using Surreal.Assets;
using Surreal.IO;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// Represents the underlying platform.
/// </summary>
public interface IPlatform
{
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

  void RegisterServices(IServiceRegistry services);
  void RegisterAssetLoaders(IAssetManager manager);
  void RegisterFileSystems(IFileSystemRegistry registry);

  void BeginFrame(TimeDelta deltaTime);
  void EndFrame(TimeDelta deltaTime);
}

/// <summary>
/// Indicates an error in the platform error of the application.
/// </summary>
public class PlatformException : Exception
{
  public PlatformException(string message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
