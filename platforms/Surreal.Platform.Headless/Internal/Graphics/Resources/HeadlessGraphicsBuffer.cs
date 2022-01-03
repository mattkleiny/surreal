using Surreal.Graphics.Meshes;

namespace Surreal.Internal.Graphics.Resources;

internal sealed class HeadlessGraphicsBuffer<T> : GraphicsBuffer<T>
  where T : unmanaged
{
  public override Memory<T> Read(Optional<Range> range = default)
  {
    return Memory<T>.Empty;
  }

  public override void Write(ReadOnlySpan<T> data)
  {
    // no-op
  }
}
