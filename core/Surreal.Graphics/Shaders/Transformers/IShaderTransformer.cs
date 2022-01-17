using static Surreal.Graphics.Shaders.ShaderSyntaxTree;

namespace Surreal.Graphics.Shaders.Transformers;

/// <summary>Transforms <see cref="ShaderDeclaration"/>s into different forms.</summary>
public interface IShaderTransformer
{
  bool CanTransform(CompilationUnit compilationUnit);

  /// <summary>Transforms the given shader, returning it's updated counterpart.</summary>
  ValueTask<CompilationUnit> TransformAsync(
    CompilationUnit compilationUnit,
    CancellationToken cancellationToken = default
  );
}
