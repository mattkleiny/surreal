﻿namespace Surreal.Build.Assets;

/// <summary>
/// Possible targets for the <see cref="IAssetBaker"/> to bake to.
/// </summary>
public readonly record struct AssetBakingTarget(string Name, string Path)
{
  public static AssetBakingTarget Desktop { get; } = new("Desktop", "desktop");
  public static AssetBakingTarget Mobile { get; } = new("Mobile", "mobile");
  public static AssetBakingTarget Web { get; } = new("Web", "web");
}

/// <summary>
/// Represents a kind of asset that can be baked with an <see cref="IAssetBaker"/>.
/// </summary>
public interface IBakeableAsset
{
  /// <summary>
  /// The ID of the asset.
  /// </summary>
  Guid AssetId { get; }

  /// <summary>
  /// The type ID of the asset.
  /// </summary>
  Guid TypeId { get; }

  /// <summary>
  /// True if the asset is embedded in the game executable.
  /// </summary>
  bool IsEmbedded { get; }
}

/// <summary>
/// A service for baking assets into a target platform's preferred format.
/// </summary>
public interface IAssetBaker
{
  Task BakeAssetsAsync(
    IEnumerable<IBakeableAsset> assets,
    AssetBakingTarget target,
    string outputPath,
    CancellationToken cancellationToken = default);
}
