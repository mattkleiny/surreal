namespace Surreal.Graphics.UI;

public interface IImmediateModeContext
{
}

public sealed class ImmediateModeContext : IImmediateModeContext, IDisposable
{
  private readonly ImmediateModeBatch batch;

  public ImmediateModeContext(IGraphicsServer server)
  {
    batch = new ImmediateModeBatch(server);
  }

  public void Dispose()
  {
    batch.Dispose();
  }
}

/// <summary>A batch of geometry that can be submitted to the GPU for use in immediate mode UI rendering.</summary>
internal sealed class ImmediateModeBatch : IDisposable
{
  public ImmediateModeBatch(IGraphicsServer server)
  {
  }

  public void Dispose()
  {
  }
}
