using Surreal.IO;

namespace Surreal.Graphics.Shaders;

/// <summary>Represents a compilation back-end for shader programs.</summary>
public interface IShaderCompiler
{
  /// <summary>Compiles the given <see cref="ShaderDeclaration"/> for use in the runtime.</summary>
  ValueTask<ICompiledShaderProgram> CompileAsync(
    IShaderCompilerEnvironment environment,
    ShaderDeclaration declaration,
    CancellationToken cancellationToken = default
  );
}

/// <summary>Represents a shader program that has been compiled from source.</summary>
public interface ICompiledShaderProgram
{
  /// <summary>The original path to the program.</summary>
  string Path { get; }
}

/// <summary>Environmental access for the <see cref="IShaderCompiler"/>.</summary>
public interface IShaderCompilerEnvironment
{
  /// <summary>Expands the given related shader back through the compilation pipeline.</summary>
  ValueTask<ShaderDeclaration> ExpandShaderAsync(VirtualPath path, CancellationToken cancellationToken = default);
}

/// <summary>A standard <see cref="IShaderCompilerEnvironment"/> that delegates back to the given <see cref="IShaderParser"/> and caches the result.</summary>
public sealed class ShaderCompilerEnvironment : IShaderCompilerEnvironment
{
  private readonly ConcurrentDictionary<VirtualPath, ShaderDeclaration> declarationsByPath = new();

  private readonly IShaderParser parser;

  public ShaderCompilerEnvironment(IShaderParser parser)
  {
    this.parser = parser;
  }

  public async ValueTask<ShaderDeclaration> ExpandShaderAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    if (!declarationsByPath.TryGetValue(path, out var declaration))
    {
      declarationsByPath[path] = declaration = await parser.ParseShaderAsync(path, cancellationToken);
    }

    return declaration;
  }
}
