using Path = Surreal.IO.Path;

namespace Surreal.Assets;

/// <summary>Represents uniquely some asset type at a given path.</summary>
public readonly record struct AssetId(Type Type, Path Path)
{
  public override string ToString() => Path.ToString();
}
