using Surreal.Graphics.Shaders;
using Surreal.Resources;

namespace Surreal.Graphics.Materials;

/// <summary>
/// The <see cref="ResourceLoader{T}" /> for <see cref="Material" />s.
/// </summary>
public sealed class MaterialLoader : ResourceLoader<Material>
{
  public override async Task<Material> LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    var shaderProgram = await context.LoadAsync<ShaderProgram>(context.Path, cancellationToken);

    return new Material(shaderProgram);
  }
}
