namespace Surreal.Graphics.Shaders;

/// <summary>Represents a parser front-end for shader programs.</summary>
public interface IShaderParser
{
  /// <summary>Parses a shader from the given <see cref="Stream"/>.</summary>
  Task<ShaderProgramDeclaration> ParseShaderAsync(string name, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return ParseShaderAsync(name, reader, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  Task<ShaderProgramDeclaration> ParseShaderAsync(string name, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return ParseShaderAsync(name, reader, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  Task<ShaderProgramDeclaration> ParseShaderAsync(string name, TextReader reader, CancellationToken cancellationToken = default);
}
