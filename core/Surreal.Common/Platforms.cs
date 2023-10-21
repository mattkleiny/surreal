using Surreal.IO;
using Surreal.Resources;
using Surreal.Timing;
using Surreal.Utilities;

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
  void RegisterAssetLoaders(IResourceManager manager);
  void RegisterFileSystems(IFileSystemRegistry registry);

  void BeginFrame(DeltaTime deltaTime);
  void EndFrame(DeltaTime deltaTime);
}

/// <summary>
/// Indicates an error in the platform error of the application.
/// </summary>
public class PlatformException(string message, Exception? innerException = null) : ApplicationException(message, innerException);
