using System.Collections.Generic;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials
{
  // TODO: abstract over shader sources?
  // TODO: replace usages of ShaderProgram with Material where appropriate
  // TODO: support batching materials along their 'shader program' usages to minimize the number of state changes

  public sealed class Material
  {
    public List<Pass>         Passes     { get; } = new List<Pass>();
    public MaterialParameters Parameters { get; } = new MaterialParameters();

    public sealed class Pass
    {
      public ShaderProgram Shader   { get; }
      public List<Texture> Textures { get; } = new List<Texture>();

      public Pass(ShaderProgram shader)
      {
        Shader = shader;
      }
    }
  }
}