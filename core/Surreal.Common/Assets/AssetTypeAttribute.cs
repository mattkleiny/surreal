namespace Surreal.Assets;

/// <summary>
/// Indicates the associated type is an asset.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AssetTypeAttribute(string id) : Attribute
{
  /// <summary>
  /// The unique identifier for this asset type.
  /// </summary>
  public Guid Id { get; } = Guid.Parse(id);
}