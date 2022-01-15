namespace Surreal.Graphics.Shaders;

/// <summary>Transforms <see cref="ShaderDeclaration"/>s into different forms.</summary>
public interface IShaderTransformer
{
  bool CanTransform(ShaderDeclaration declaration);

  /// <summary>Transforms the given shader, returning it's updated counterpart.</summary>
  ValueTask<ShaderDeclaration> TransformAsync(
    ShaderDeclaration declaration,
    CancellationToken cancellationToken = default
  );
}
