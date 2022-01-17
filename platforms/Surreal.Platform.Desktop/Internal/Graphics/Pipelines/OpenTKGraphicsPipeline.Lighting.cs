using Surreal.Graphics.Pipelines;
using static Surreal.Graphics.Pipelines.IGraphicsPipeline;

namespace Surreal.Internal.Graphics.Pipelines;

public sealed partial class OpenTKGraphicsPipeline : ILighting
{
  public ILighting Lighting => this;

  public GraphicsId CreateLight()
  {
    throw new NotImplementedException();
  }

  public void SetTransform(GraphicsId id, in Matrix4x4 transform)
  {
    throw new NotImplementedException();
  }

  public void SetShadowTransform(GraphicsId id, in Matrix4x4 transform)
  {
    throw new NotImplementedException();
  }
}
