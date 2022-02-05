namespace Surreal.Graphics.Shaders;

/// <summary>A compiler for <see cref="ShaderDeclaration"/>s.</summary>
public interface IShaderCompiler
{
  /// <summary>Compiles the given shader declaration into a program usable by the graphics server.</summary>
  ICompiledShader CompileShader(ShaderDeclaration declaration);
}

/// <summary>An opaque compiled shader representation.</summary>
public interface ICompiledShader
{
}
