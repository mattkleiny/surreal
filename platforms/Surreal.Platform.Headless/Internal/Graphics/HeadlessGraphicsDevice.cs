using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessGraphicsDevice : IGraphicsDevice
{
  public IShaderCompiler   ShaderCompiler { get; } = new HeadlessShaderCompiler();
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

  public Texture CreateTexture(
    TextureFormat format = TextureFormat.Rgba8888,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  )
  {
    return new HeadlessTexture(format, filterMode, wrapMode);
  }

  public Texture CreateTexture(
    ITextureData data,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  )
  {
    return new HeadlessTexture(data.Format, filterMode, wrapMode);
  }

  public RenderTexture CreateFrameBuffer(in RenderTextureDescriptor descriptor)
  {
    return new HeadlessRenderTexture();
  }

  public ShaderProgram CreateShaderProgram(ICompiledShaderProgram program)
  {
    return new HeadlessShaderProgram();
  }
}
