namespace Surreal.Assets;

/// <summary>
/// Metadata about how to read and process an asset.
/// </summary>
public sealed record AssetMetadata
{
  /// <summary>
  /// A unique identifier for this asset.
  /// </summary>
  public required Guid AssetId { get; init; }

  /// <summary>
  /// The type of the asset.
  /// </summary>
  public required Guid TypeId { get; init; }

  /// <summary>
  /// The properties of the asset.
  /// </summary>
  public Dictionary<string, object> Properties { get; init; } = new();
}
