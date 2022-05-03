using Surreal.Assets;
using Surreal.Graphics.Shaders;
using Surreal.IO;

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

  public override async Task<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var shaderSet = await LoadShaderSetAsync(context, cancellationToken);
    var program = new ShaderProgram(server);

    server.LinkShader(program.Handle, shaderSet);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<ShaderProgram>(ReloadAsync);
    }

    return program;
  }

  private async Task<ShaderProgram> ReloadAsync(AssetLoaderContext context, ShaderProgram program, CancellationToken cancellationToken = default)
  {
    var shaderSet = await LoadShaderSetAsync(context, cancellationToken);
    var handle = server.CreateShader();

    server.LinkShader(handle, shaderSet);
    program.ReplaceShader(handle);

    return program;
  }

  private static async Task<OpenTKShaderSet> LoadShaderSetAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream, Encoding.UTF8);

    return await OpenTKShaderProcessor.ProcessGlslCodeAsync(context.Path, reader, cancellationToken);
  }
}
