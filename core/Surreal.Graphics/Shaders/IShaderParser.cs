namespace Surreal.Graphics.Shaders;

/// <summary>Represents a parser front-end for shader programs.</summary>
public interface IShaderParser
{
  /// <summary>Parses a shader from the given <see cref="Stream"/>.</summary>
  ValueTask<ShaderProgramDeclaration> ParseShaderAsync(string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return ParseShaderAsync(path, reader, (int) stream.Length, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  ValueTask<ShaderProgramDeclaration> ParseShaderAsync(string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return ParseShaderAsync(path, reader, sourceCode.Length, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  ValueTask<ShaderProgramDeclaration> ParseShaderAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default);
}
