using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Internal.Graphics.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessGraphicsDevice : IGraphicsDevice
{
  public IGraphicsServer Server       => throw new NotImplementedException();
  public Viewport          Viewport       { get; set; }

  public void BeginFrame()
  {
    // no-op
  }

  public void Clear(Color color)
  {
    // no-op
  }

  public void ClearColor(Color color)
  {
    // no-op
  }

  public void ClearDepth()
  {
    // no-op
  }

  public void DrawMesh(
    Mesh mesh,
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles
  )
  {
    // no-op
  }

  public void EndFrame()
  {
    // no-op
  }

  public void Present()
  {
    // no-op
  }

  public GraphicsBuffer<T> CreateBuffer<T>()
    where T : unmanaged
  {
    return new HeadlessGraphicsBuffer<T>();
  }
}
