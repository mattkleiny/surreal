using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics;

/// <summary>Base class for any graphical resource.</summary>
public abstract class GraphicsResource : TrackedResource<GraphicsResource>
{
  public static Size AllocatedBufferSize  => GetSizeEstimate<GraphicsBuffer>();
  public static Size AllocatedTextureSize => GetSizeEstimate<Texture>();
}
