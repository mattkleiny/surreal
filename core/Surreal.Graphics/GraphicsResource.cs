using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics;

/// <summary>
/// Base class for any graphical assets
/// </summary>
public abstract class GraphicsAsset : TrackedAsset<GraphicsAsset>
{
  public static Size TotalBufferSize => GetSizeEstimate<GraphicsBuffer>();
  public static Size TotalTextureSize => GetSizeEstimate<Texture>();
  public static Size TotalMeshSize => GetSizeEstimate<Mesh>();
}
