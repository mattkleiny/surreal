using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Pipelines;
using Surreal.Graphics.Shaders;
using static Surreal.Graphics.Pipelines.IGraphicsPipeline;

namespace Surreal.Internal.Graphics.Pipelines;

public sealed partial class OpenTKGraphicsPipeline : IShaders
{
  public IShaders Shaders => this;

  public GraphicsId CreateShader()
  {
    var shader = GL.CreateShader(ShaderType.VertexShader);

    return new GraphicsId(shader.Handle);
  }

  public void Compile(GraphicsId id, ShaderDeclaration declaration)
  {
    var handle = new ShaderHandle(id);

    GL.CompileShader(handle);

    throw new NotImplementedException();
  }
}
