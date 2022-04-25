using AutoFixture.Kernel;

namespace Surreal.Graphics;

[RegisterSpecimenBuilder]
internal sealed class HeadlessGraphicsServerBuilder : SpecimenBuilder<IGraphicsServer>
{
  protected override IGraphicsServer Create(ISpecimenContext context, string? name = null)
  {
    return new HeadlessGraphicsServer();
  }
}
