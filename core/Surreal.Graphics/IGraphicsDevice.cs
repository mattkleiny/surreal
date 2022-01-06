using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A viewport for scissoring operations on a viewport.</summary>
public readonly record struct Viewport(int X, int Y, int Width, int Height);

/// <summary>Represents the underlying graphics device.</summary>
public interface IGraphicsDevice
{
  IShaderCompiler ShaderCompiler { get; }
  Viewport        Viewport       { get; set; }

  void Clear(Color color);
  void ClearColor(Color color);
  void ClearDepth();

  void DrawMesh<TVertex>(
    Mesh<TVertex> mesh,
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles)
    where TVertex : unmanaged;

  void BeginFrame();
  void EndFrame();
  void Present();

  GraphicsBuffer<T> CreateBuffer<T>()
    where T : unmanaged;

  Texture CreateTexture(
    TextureFormat format = TextureFormat.Rgba8888,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  );

  Texture CreateTexture(
    ITextureData data,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  );

  RenderTexture CreateFrameBuffer(in RenderTextureDescriptor descriptor);
  ShaderProgram CreateShaderProgram(ICompiledShaderProgram program);
}
