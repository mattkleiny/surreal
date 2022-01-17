using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A viewport for scissoring operations on a viewport.</summary>
public readonly record struct Viewport(int X, int Y, int Width, int Height);

/// <summary>Represents the underlying graphics device.</summary>
public interface IGraphicsDevice
{
  IGraphicsServer Server   { get; }
  Viewport        Viewport { get; set; }

  void Clear(Color color);
  void ClearColor(Color color);
  void ClearDepth();

  void DrawMesh(
    Mesh mesh,
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles
  );

  void BeginFrame();
  void EndFrame();
  void Present();

  GraphicsBuffer<T> CreateBuffer<T>()
    where T : unmanaged;
}
