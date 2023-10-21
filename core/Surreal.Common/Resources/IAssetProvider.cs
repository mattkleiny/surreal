using Surreal.IO;

namespace Surreal.Resources;

/// <summary>
/// Provides accesses to application <see cref="Asset"/>s.
/// </summary>
public interface IAssetProvider
{
  /// <summary>
  /// Loads an asset from the given path.
  /// </summary>
  Task<T> LoadAssetAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);
}
