namespace Surreal.Graphics.Shaders;

/// <summary>Represents a parser front-end for <see cref="IParsedShader"/>s.</summary>
public interface IShaderParser
{
  /// <summary>Parses a shader from the given <see cref="Stream"/>.</summary>
  Task<IParsedShader> ParseShaderAsync(Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return ParseShaderAsync(reader, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  Task<IParsedShader> ParseShaderAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    var source = new TextShaderSource(reader);

    return ParseShaderAsync(source, cancellationToken);
  }

  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  Task<IParsedShader> ParseShaderAsync(string source, CancellationToken cancellationToken = default)
  {
    return ParseShaderAsync(new StringReader(source), cancellationToken);
  }

  /// <summary>Parses the given <see cref="IShaderSource"/> for use in compilation.</summary>
  Task<IParsedShader> ParseShaderAsync(IShaderSource source, CancellationToken cancellationToken = default);

  /// <summary>A <see cref="IShaderSource"/> based on a <see cref="TextReader"/>.</summary>
  private sealed class TextShaderSource : IShaderSource
  {
    private readonly TextReader reader;

    public TextShaderSource(TextReader reader)
    {
      this.reader = reader;
    }
  }
}

/// <summary>Abstracts over different possible sources for shader programs.</summary>
public interface IShaderSource
{
}

/// <summary>Represents a parsed shader, ready for interrogation and compilation.</summary>
public interface IParsedShader
{
}
