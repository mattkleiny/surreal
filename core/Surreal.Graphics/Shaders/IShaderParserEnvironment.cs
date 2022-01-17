using Surreal.IO;

namespace Surreal.Graphics.Shaders;

/// <summary>Environmental access for the <see cref="IShaderCompiler"/>.</summary>
public interface IShaderParserEnvironment
{
  /// <summary>Expands the given related shader back through the compilation pipeline.</summary>
  ValueTask<ShaderDeclaration> ExpandShaderAsync(
    IShaderParser parser,
    VirtualPath path,
    CancellationToken cancellationToken = default
  );
}

/// <summary>A standard <see cref="IShaderParserEnvironment"/> that delegates back to the given <see cref="IShaderParser"/> and caches the result.</summary>
public sealed class ShaderParserEnvironment : IShaderParserEnvironment
{
  private readonly ConcurrentDictionary<VirtualPath, ShaderDeclaration> declarationsByPath = new();

  public async ValueTask<ShaderDeclaration> ExpandShaderAsync(IShaderParser parser, VirtualPath path, CancellationToken cancellationToken = default)
  {
    if (!declarationsByPath.TryGetValue(path, out var declaration))
    {
      declarationsByPath[path] = declaration = await parser.ParseShaderAsync(path, this, cancellationToken);
    }

    return declaration;
  }
}
