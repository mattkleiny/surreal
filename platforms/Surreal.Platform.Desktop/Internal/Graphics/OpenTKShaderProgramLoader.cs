using OpenTK.Graphics.OpenGL;
using Surreal.Assets;
using Surreal.Graphics.Shaders;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

/// <summary>A single shader, unlinked to a program.</summary>
internal sealed record OpenTKShader(ShaderType Type, string Code);

/// <summary>A set of <see cref="OpenTKShader" />s.</summary>
internal sealed record OpenTKShaderSet(string Path, ImmutableArray<OpenTKShader> Shaders);

/// <summary>The <see cref="AssetLoader{T}" /> for GLSL <see cref="ShaderProgram" />s.</summary>
internal sealed class OpenTKShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly OpenTKGraphicsServer _server;

  public OpenTKShaderProgramLoader(OpenTKGraphicsServer server)
  {
    _server = server;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && context.Path.Extension == ".glsl";
  }

  public override async Task<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var shaderSet = await LoadShaderSetAsync(context, cancellationToken);
    var program = new ShaderProgram(_server);

    _server.LinkShader(program.Handle, shaderSet);
    program.ReloadMetadata();

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<ShaderProgram>(ReloadAsync);
    }

    return program;
  }

  private async Task<ShaderProgram> ReloadAsync(AssetLoaderContext context, ShaderProgram program, CancellationToken cancellationToken = default)
  {
    var shaderSet = await LoadShaderSetAsync(context, cancellationToken);
    var handle = _server.CreateShader();

    _server.LinkShader(handle, shaderSet);
    program.ReplaceShader(handle);
    program.ReloadMetadata();

    return program;
  }

  private static async Task<OpenTKShaderSet> LoadShaderSetAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    await using var stream = await context.Path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream, Encoding.UTF8);

    return await ParseCodeAsync(context.Path, reader, cancellationToken);
  }

  /// <summary>
  ///   Processes a GLSL program in the given <see cref="TextReader" /> and pre processes it with some useful features.
  ///   In particular:
  ///   * Allow multiple shader kernels per file via a #shader_type directive.
  /// </summary>
  [VisibleForTesting]
  internal static async Task<OpenTKShaderSet> ParseCodeAsync(VirtualPath path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var sharedCode = new StringBuilder();
    var shaderCode = new List<Shader>();

    await foreach (var line in reader.ReadLinesAsync(cancellationToken))
    {
      if (line.Trim().StartsWith("#shader_type"))
      {
        if (line.EndsWith("vertex"))
        {
          shaderCode.Add(new Shader(ShaderType.VertexShader, new StringBuilder(sharedCode.ToString())));
        }

        if (line.EndsWith("fragment"))
        {
          shaderCode.Add(new Shader(ShaderType.FragmentShader, new StringBuilder(sharedCode.ToString())));
        }
      }
      else if (shaderCode.Count > 0)
      {
        shaderCode[^1].Code.AppendLine(line);
      }
      else
      {
        sharedCode.AppendLine(line);
      }
    }

    return new OpenTKShaderSet(path.ToString(), shaderCode.Select(_ => new OpenTKShader(_.Type, _.Code.ToString())).ToImmutableArray());
  }

  /// <summary>A mutable version of the <see cref="OpenTKShader" /> that we can build up in stages.</summary>
  private readonly record struct Shader(ShaderType Type, StringBuilder Code);
}


