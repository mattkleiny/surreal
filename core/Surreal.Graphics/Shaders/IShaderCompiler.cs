namespace Surreal.Graphics.Shaders;

/// <summary>Represents a compilation back-end for shader programs.</summary>
public interface IShaderCompiler
{
  /// <summary>Compiles the given <see cref="ShaderProgramDeclaration"/> for use in the runtime.</summary>
  ValueTask<ICompiledShaderProgram> CompileAsync(ShaderProgramDeclaration declaration);
}

/// <summary>Represents a shader program that has been compiled from source.</summary>
public interface ICompiledShaderProgram
{
  /// <summary>The original path to the program.</summary>
  string Path { get; }
}
