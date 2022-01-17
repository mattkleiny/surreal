using Surreal.Graphics.Pipelines;
using static Surreal.Graphics.Pipelines.IGraphicsPipeline;

namespace Surreal.Internal.Graphics.Pipelines;

public sealed partial class OpenTKGraphicsPipeline : IBuffers
{
  public IBuffers Buffers => this;

  public GraphicsId CreateBuffer()
  {
    throw new NotImplementedException();
  }

  public void UploadData<T>(GraphicsId buffers, ReadOnlySpan<T> data)
  {
    throw new NotImplementedException();
  }
}
