using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

/// <summary>A utility for pre-processing GLSL shader code.</summary>
internal static class OpenTKShaderProcessor
{
  public static async Task<OpenTKShaderSet> ProcessGlslCodeAsync(VirtualPath path, TextReader reader, CancellationToken cancellationToken = default)
  {
    // TODO: make ordering more robust?

    var sharedCode = await ParseSharedCodeAsync(reader, cancellationToken);
    var vertexCode = await ParseVertexCodeAsync(reader, cancellationToken);
    var fragmentCode = await ParseFragmentCodeAsync(reader, cancellationToken);

    var shaders = ImmutableArray.Create(
      new OpenTKShader(ShaderType.VertexShader, sharedCode + vertexCode.ToString()),
      new OpenTKShader(ShaderType.FragmentShader, sharedCode + fragmentCode.ToString())
    );

    return new OpenTKShaderSet(path.ToString(), shaders);
  }

  private static async Task<StringBuilder> ParseSharedCodeAsync(TextReader reader, CancellationToken cancellationToken)
  {
    var builder = new StringBuilder();

    await foreach (var line in ReadLinesAsync(reader, cancellationToken))
    {
      if (line.Contains("#shader_type vertex"))
      {
        return builder;
      }

      builder.AppendLine(line);
    }

    return builder;
  }

  private static async Task<StringBuilder> ParseVertexCodeAsync(TextReader reader, CancellationToken cancellationToken)
  {
    var builder = new StringBuilder();

    await foreach (var line in ReadLinesAsync(reader, cancellationToken))
    {
      if (line.Contains("#shader_type fragment"))
      {
        return builder;
      }

      builder.AppendLine(line);
    }

    return builder;
  }

  private static async Task<StringBuilder> ParseFragmentCodeAsync(TextReader reader, CancellationToken cancellationToken)
  {
    var builder = new StringBuilder();

    await foreach (var line in ReadLinesAsync(reader, cancellationToken))
    {
      builder.AppendLine(line);
    }

    return builder;
  }

  private static async IAsyncEnumerable<string> ReadLinesAsync(TextReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (true)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var line = await reader.ReadLineAsync();
      if (line == null)
      {
        yield break;
      }

      yield return line;
    }
  }
}
