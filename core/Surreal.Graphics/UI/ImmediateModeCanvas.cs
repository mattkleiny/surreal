namespace Surreal.Graphics.UI;

public interface IImmediateModeCanvas
{
}

public sealed class ImmediateModeCanvas : IImmediateModeCanvas, IDisposable
{
  private readonly ImmediateModeBatch batch;

  public ImmediateModeCanvas(IGraphicsServer server)
  {
    batch = new ImmediateModeBatch(server);
  }

  public void Dispose()
  {
    batch.Dispose();
  }
}
