using Surreal.Assets;

namespace Surreal.Graphics.Materials;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="ShaderProgram" />s.
/// </summary>
public sealed class ShaderProgramLoader(IGraphicsBackend backend) : AssetLoader<ShaderProgram>
{
  public override async Task<ShaderProgram> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    return await ShaderProgram.LoadAsync(backend, context.Path, cancellationToken);
  }
}
