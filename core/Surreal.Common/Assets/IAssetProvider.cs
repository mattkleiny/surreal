﻿using Surreal.IO;

namespace Surreal.Assets;

/// <summary>
/// Provides accesses to application <see cref="Disposable"/>s.
/// </summary>
public interface IAssetProvider
{
  /// <summary>
  /// Loads an asset from the given path.
  /// </summary>
  Task<Asset<T>> LoadAsync<T>(VirtualPath path, CancellationToken cancellationToken = default);
}
