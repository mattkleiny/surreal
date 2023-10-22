namespace Surreal.Graphics.Materials;

/// <summary>
/// Different types of shaders.
/// </summary>
public enum ShaderType
{
  VertexShader,
  FragmentShader,
}

/// <summary>
/// A set of shader code that can be used to link a <see cref="ShaderProgram"/>.
/// </summary>
public readonly record struct ShaderKernel(ShaderType Type, StringBuilder Code);
