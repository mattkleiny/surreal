using Surreal.Assets;
using Surreal.Graphics.Shaders;

namespace Surreal.Editor.Importers;

/// <summary>
/// Imports <see cref="ShaderProgram"/>s from GLSL files.
/// </summary>
internal sealed class GlslShaderProgramImporter : AssetImporter<ShaderProgram>
{
  public override bool CanHandle(string path)
  {
    return path.EndsWith(".glsl");
  }

  public override Task<ShaderProgram> ImportAsync(string path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
