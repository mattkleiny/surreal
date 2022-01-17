using Surreal.Graphics.Pipelines;
using static Surreal.Graphics.Pipelines.IGraphicsPipeline;

namespace Surreal.Internal.Graphics.Pipelines;

public sealed partial class OpenTKGraphicsPipeline : IMaterials
{
  public IMaterials Materials => this;

  public GraphicsId CreateMaterial()
  {
    throw new NotImplementedException();
  }

  public void SetShader(GraphicsId material, GraphicsId shader)
  {
    throw new NotImplementedException();
  }
}
