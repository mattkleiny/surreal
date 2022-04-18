namespace Surreal.Graphics.Shaders.Transformers;

/// <summary>Transforms <see cref="ShaderDeclaration"/>s into different forms.</summary>
public interface IShaderTransformer
{
  /// <summary>Determines if the given <see cref="ShaderCompilationUnit"/> can be transformed.</summary>
  bool CanTransform(ShaderCompilationUnit compilationUnit);

  /// <summary>Transforms the given shader, returning it's updated counterpart.</summary>
  ShaderCompilationUnit Transform(ShaderCompilationUnit compilationUnit);
}
