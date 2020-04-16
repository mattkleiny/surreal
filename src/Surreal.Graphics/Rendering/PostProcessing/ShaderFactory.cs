using System.Collections.Generic;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Rendering.PostProcessing
{
  public sealed class ShaderFactory
  {
    private readonly Dictionary<string, ShaderProgram> programs = new Dictionary<string, ShaderProgram>();
    private readonly IAssetResolver                    assets;

    public ShaderFactory(IAssetResolver assets)
    {
      this.assets = assets;
    }

    public ShaderProgram GetOrCreate(string path)
    {
      return programs.GetOrComputeAsync(path, key => assets.GetAsync<ShaderProgram>(key)).Result;
    }
  }
}