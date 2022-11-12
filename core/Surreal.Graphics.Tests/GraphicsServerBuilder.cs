using AutoFixture.Kernel;

namespace Surreal.Graphics;

[RegisterSpecimenBuilder]
internal sealed class GraphicsServerBuilder : SpecimenBuilder<IGraphicsServer>
{
  protected override IGraphicsServer Create(ISpecimenContext context, string? name = null)
  {
    return new HeadlessGraphicsServer();
  }
}





