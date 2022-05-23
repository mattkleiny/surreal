using OpenTK.Graphics.OpenGL;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

/// <summary>A utility for pre-processing GLSL shader code.</summary>
internal static class OpenTKShaderProcessor
{
  /// <summary>
  /// Processes a GLSL program in the given <see cref="TextReader"/> and pre processes it with some useful features.
  ///
  /// In particular:
  /// * Allow multiple shader kernels per file via a #shader_type directive.
  /// </summary>
  public static async Task<OpenTKShaderSet> ParseCodeAsync(VirtualPath path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var sharedCode = new StringBuilder();
    var shaderCode = new List<Shader>();

    await foreach (var line in reader.ReadLinesAsync(cancellationToken))
    {
      if (line.Trim().StartsWith("#shader_type"))
      {
        if (line.EndsWith("vertex")) shaderCode.Add(new Shader(ShaderType.VertexShader, new StringBuilder(sharedCode.ToString())));
        if (line.EndsWith("fragment")) shaderCode.Add(new Shader(ShaderType.FragmentShader, new StringBuilder(sharedCode.ToString())));
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

  /// <summary>A mutable version of the <see cref="OpenTKShader"/> that we can build up in stages.</summary>
  private readonly record struct Shader(ShaderType Type, StringBuilder Code);
}
