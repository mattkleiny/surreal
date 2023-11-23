namespace Surreal.Services;

/// <summary>
/// A service that should be initialized upon startup.
/// </summary>
public interface IInitializable
{
  /// <summary>
  /// Initializes the service.
  /// </summary>
  void Initialize();
}
