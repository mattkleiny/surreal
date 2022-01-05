namespace Surreal.Graphics.Shaders;

/// <summary>Represents a compilation back-end for <see cref="IParsedShader"/>s.</summary>
public interface IShaderCompiler
{
  /// <summary>Compiles the given <see cref="IParsedShader"/> for use in the runtime.</summary>
  Task<ICompiledShader> CompileAsync(IParsedShader shader);
}

/// <summary>Represents a shader program that has been compiled from source.</summary>
public interface ICompiledShader
{
}
