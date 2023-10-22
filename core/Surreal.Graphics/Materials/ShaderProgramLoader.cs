using Surreal.Assets;
using Surreal.Utilities;

namespace Surreal.Graphics.Materials;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="ShaderProgram" />s.
/// </summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class ShaderProgramLoader(IGraphicsBackend backend) : AssetLoader<ShaderProgram>
{
  public override Task<ShaderProgram> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
