using Surreal.Resources;

namespace Surreal.Graphics.Materials;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Material" />s.
/// </summary>
public sealed class MaterialLoader : AssetLoader<Material>
{
  public override async Task<Material> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    var shaderProgram = await context.LoadAsync<ShaderProgram>(context.Path, cancellationToken);

    return new Material(shaderProgram);
  }
}
