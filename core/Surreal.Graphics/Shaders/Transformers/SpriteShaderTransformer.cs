namespace Surreal.Graphics.Shaders.Transformers;

/// <summary>A <see cref="IShaderTransformer"/> that optimizes shader programs sprite rendering.</summary>
public sealed class SpriteShaderTransformer : IShaderTransformer
{
  public bool CanTransform(ShaderDeclaration declaration)
  {
    return declaration.CompilationUnit.ShaderType is { Type: "sprite" };
  }

  public ValueTask<ShaderDeclaration> TransformAsync(ShaderDeclaration declaration, CancellationToken cancellationToken = default)
  {
    var (path, compilationUnit) = declaration;

    // TODO: how to make this simpler?
    var fragmentStage = compilationUnit.Stages.FirstOrDefault(_ => _.Kind == ShaderKind.Fragment);
    if (fragmentStage != null)
    {
      throw new NotImplementedException();
    }

    return ValueTask.FromResult(new ShaderDeclaration(path, compilationUnit));
  }
}
