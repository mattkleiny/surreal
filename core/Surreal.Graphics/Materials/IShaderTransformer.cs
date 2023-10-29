using static Surreal.Graphics.Materials.ShaderSyntaxTree;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Transforms <see cref="ShaderDeclaration"/>s into different forms.
/// </summary>
public interface IShaderTransformer
{
  /// <summary>
  /// Determines if this transformer can transform the given shader.
  /// </summary>
  bool CanTransform(CompilationUnit compilationUnit);

  /// <summary>
  /// Transforms the given shader, returning it's updated counterpart.
  /// </summary>
  ValueTask<CompilationUnit> TransformAsync(
    CompilationUnit compilationUnit,
    CancellationToken cancellationToken = default);
}
