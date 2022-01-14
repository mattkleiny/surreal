using Surreal.IO;

namespace Surreal.Graphics.Shaders;

/// <summary>Represents a compilation back-end for shader programs.</summary>
public interface IShaderCompiler
{
  /// <summary>Compiles the given <see cref="ShaderDeclaration"/> for use in the runtime.</summary>
  ValueTask<ICompiledShaderProgram> CompileAsync(
    IShaderCompilerContext context,
    ShaderDeclaration declaration,
    CancellationToken cancellationToken = default
  );
}

/// <summary>Environmental access for the <see cref="IShaderCompiler"/>.</summary>
public interface IShaderCompilerContext
{
  /// <summary>Expands the given related shader back through the compilation pipeline.</summary>
  ValueTask<ShaderDeclaration> ExpandShaderAsync(VirtualPath path, CancellationToken cancellationToken = default);
}

/// <summary>Represents a shader program that has been compiled from source.</summary>
public interface ICompiledShaderProgram
{
  /// <summary>The original path to the program.</summary>
  string Path { get; }
}
