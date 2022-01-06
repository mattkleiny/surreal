using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Images;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;
using Surreal.Mathematics;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTkGraphicsDevice : IGraphicsDevice
{
  private readonly IDesktopWindow window;

  public OpenTkGraphicsDevice(IDesktopWindow window)
  {
    this.window = window;
  }

  public IShaderCompiler ShaderCompiler { get; } = new OpenTkShaderCompiler();
  public Viewport        Viewport       { get; set; }

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

  public void DrawMesh<TVertex>(
    Mesh<TVertex> mesh,
    Material material,
    int vertexCount,
    int indexCount,
    MeshType type = MeshType.Triangles)
    where TVertex : unmanaged
  {
    if (vertexCount == 0) return; // empty mesh? don't render

    // TODO: bind shader and buffers

    material.Program.Bind(mesh.Descriptors);

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

  public GraphicsBuffer<T> CreateBuffer<T>()
    where T : unmanaged
  {
    return new OpenTkGraphicsBuffer<T>();
  }

  public Texture CreateTexture(
    TextureFormat format = TextureFormat.Rgba8888,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  )
  {
    return new OpenTkTexture(format, filterMode, wrapMode);
  }

  public Texture CreateTexture(
    ITextureData data,
    TextureFilterMode filterMode = TextureFilterMode.Linear,
    TextureWrapMode wrapMode = TextureWrapMode.Repeat
  )
  {
    return new OpenTkTexture(data, filterMode, wrapMode);
  }

  public RenderTexture CreateFrameBuffer(in RenderTextureDescriptor descriptor)
  {
    var texture = (OpenTkTexture) CreateTexture(descriptor.Format, descriptor.FilterMode, TextureWrapMode.Clamp);
    var image   = new Image(descriptor.Width, descriptor.Height);

    texture.Upload(image);

    return new OpenTkRenderTexture(texture, image);
  }

  public ShaderProgram CreateShaderProgram(ICompiledShaderProgram program)
  {
    var shaderSet = (OpenTkShaderSet) program;

    return new OpenTkShaderProgram(shaderSet);
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
