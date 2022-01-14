using Surreal.IO;

namespace Surreal.Graphics.Shaders;

/// <summary>Represents a parser front-end for shader programs.</summary>
public interface IShaderParser
{
  /// <summary>Parses a shader from the given <see cref="TextReader"/>.</summary>
  ValueTask<ShaderDeclaration> ParseShaderAsync(string path, TextReader reader, CancellationToken cancellationToken = default);
}

/// <summary>Static extensions for <see cref="IShaderParser"/>s.</summary>
public static class ShaderParserExtensions
{
  public static ValueTask<ShaderDeclaration> ParseShaderAsync(this IShaderParser parser, VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ParseShaderAsync(parser, path, Encoding.UTF8, cancellationToken);
  }

  public static async ValueTask<ShaderDeclaration> ParseShaderAsync(this IShaderParser parser, VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    return await ParseShaderAsync(parser, path.ToString(), stream, encoding, cancellationToken);
  }

  public static ValueTask<ShaderDeclaration> ParseShaderAsync(this IShaderParser parser, string path, Stream stream, CancellationToken cancellationToken = default)
  {
    return ParseShaderAsync(parser, path, stream, Encoding.UTF8, cancellationToken);
  }

  public static async ValueTask<ShaderDeclaration> ParseShaderAsync(this IShaderParser parser, string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return await parser.ParseShaderAsync(path, reader, cancellationToken);
  }

  public static async ValueTask<ShaderDeclaration> ParseShaderAsync(this IShaderParser parser, string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return await parser.ParseShaderAsync(path, reader, cancellationToken);
  }
}
