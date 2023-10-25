namespace Surreal.Assets;

/// <summary>
/// Allows a type to be hot-reloaded by the asset system.
/// </summary>
public interface IHotReloadable<T>
{
  /// <summary>
  /// Invoked when an asset has been reloaded by the manager.
  /// </summary>
  void OnHotReload(T asset);
}
