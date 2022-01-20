using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Internal.Graphics.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTKGraphicsDevice : IGraphicsDevice
{
  private readonly IDesktopWindow window;

  public OpenTKGraphicsDevice(IDesktopWindow window)
  {
    this.window = window;
  }

  public IGraphicsServer Server   { get; } = new OpenTKGraphicsServer();
  public Viewport        Viewport { get; set; }

  public void BeginFrame()
  {
    // no-op
  }

  public void Clear(Color color)
  {
    ClearDepth();
    ClearColor(color);
  }

  public void ClearColor(Color color)
  {
    GL.ClearColor(color.R, color.G, color.B, color.A);
    GL.Clear(ClearBufferMask.ColorBufferBit);
  }

  public void ClearDepth()
  {
    GL.Clear(ClearBufferMask.DepthBufferBit);
  }

  public void DrawMesh(
    Mesh mesh,
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles
  )
  {
    if (vertexCount == 0) return; // empty mesh? don't render

    // TODO: bind shader and buffers

    if (indexCount > 0)
    {
      DrawMeshIndexed(indexCount, type);
    }
    else
    {
      DrawMesh(vertexCount, type);
    }
  }

  public void Present()
  {
    window.Present();
  }

  public void EndFrame()
  {
    GL.Flush();
  }

  [Obsolete("Use the graphics server, instead")]
  public GraphicsBuffer<T> CreateBuffer<T>()
    where T : unmanaged
  {
    return new OpenTKGraphicsBuffer<T>();
  }

  private static void DrawMesh(int count, MeshType type)
  {
    Debug.Assert(count >= 0, "count >= 0");

    GL.DrawArrays(ConvertPrimitiveType(type), 0, count);
  }

  private static void DrawMeshIndexed(int count, MeshType type)
  {
    Debug.Assert(count >= 0, "count >= 0");

    GL.DrawElements(ConvertPrimitiveType(type), count, DrawElementsType.UnsignedShort, IntPtr.Zero);
  }

  private static PrimitiveType ConvertPrimitiveType(MeshType type) => type switch
  {
    MeshType.Points    => PrimitiveType.Points,
    MeshType.Lines     => PrimitiveType.Lines,
    MeshType.LineStrip => PrimitiveType.LineStrip,
    MeshType.LineLoop  => PrimitiveType.LineLoop,
    MeshType.Triangles => PrimitiveType.Triangles,
    MeshType.Quads     => PrimitiveType.Quads,
    MeshType.QuadStrip => PrimitiveType.QuadStrip,
    _                  => throw new ArgumentOutOfRangeException(nameof(type), type, "An unrecognized primitive type was requested."),
  };
}
