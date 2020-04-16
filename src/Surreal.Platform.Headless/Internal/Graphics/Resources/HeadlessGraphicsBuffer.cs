using System;
using Surreal.Graphics.Meshes;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  internal sealed class HeadlessGraphicsBuffer : GraphicsBuffer
  {
    public HeadlessGraphicsBuffer(int stride)
      : base(stride)
    {
    }

    public override void Put<T>(Span<T> data)
    {
      // no-op
    }
  }
}
