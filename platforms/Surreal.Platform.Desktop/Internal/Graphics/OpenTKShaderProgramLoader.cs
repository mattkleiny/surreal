using OpenTK.Graphics.OpenGL;
using Surreal.Assets;
using Surreal.Graphics.Shaders;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal.Internal.Graphics;

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="OpenTKShaderSet"/>s.</summary>
internal sealed class OpenTKShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly OpenTKGraphicsServer server;

  public OpenTKShaderProgramLoader(OpenTKGraphicsServer server)
  {
    this.server = server;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && context.Path.Extension == ".glsl";
  }

  public override async ValueTask<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var vertexPath = context.Path.ChangeExtension("vert.glsl");
    var fragmentPath = context.Path.ChangeExtension("frag.glsl");

    var vertexCode = await vertexPath.ReadAllTextAsync(Encoding.UTF8, cancellationToken);
    var fragmentCode = await fragmentPath.ReadAllTextAsync(Encoding.UTF8, cancellationToken);

    await Task.Delay(1.Seconds());

    var shaders = ImmutableArray.Create(
      new OpenTKShader(ShaderType.VertexShader, vertexCode),
      new OpenTKShader(ShaderType.FragmentShader, fragmentCode)
    );

    var shaderSet = new OpenTKShaderSet(context.Path.ToString(), shaders);
    var program = new ShaderProgram(server);

    server.LinkShader(program.Handle, shaderSet);

    return program;
  }
}
