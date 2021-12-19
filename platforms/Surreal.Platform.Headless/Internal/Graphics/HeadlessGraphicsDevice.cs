using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessGraphicsDevice : IGraphicsDevice
{
  public IPipelineState Pipeline { get; } = new HeadlessPipelineState();

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

  public void DrawMesh<TVertex>(
    Mesh<TVertex> mesh,
    Material material,
    int vertexCount,
    int indexCount,
    PrimitiveType type = PrimitiveType.Triangles)
    where TVertex : unmanaged
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

  public GraphicsBuffer<T> CreateBuffer<T>() where T : unmanaged
  {
    return new HeadlessGraphicsBuffer<T>();
  }

  public Texture CreateTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    return new HeadlessTexture(format, filterMode, wrapMode);
  }

  public Texture CreateTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    return new HeadlessTexture(data.Format, filterMode, wrapMode);
  }

  public RenderTexture CreateFrameBuffer(in RenderTextureDescriptor descriptor)
  {
    return new HeadlessRenderTexture();
  }
}