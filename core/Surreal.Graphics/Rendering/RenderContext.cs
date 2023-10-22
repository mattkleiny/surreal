namespace Surreal.Graphics.Rendering;

/// <summary>
/// Allows for context-specific rendering.
/// </summary>
public interface IRenderContext
{
  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);

  void OnBeginScope(in RenderFrame frame);
  void OnEndScope(in RenderFrame frame);
}

/// <summary>
/// Convenience class for <see cref="IRenderContext"/>.
/// </summary>
public abstract class RenderContext : IRenderContext, IDisposable
{
  protected internal virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  protected internal virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  protected internal virtual void OnBeginScope(in RenderFrame frame)
  {
  }

  protected internal virtual void OnEndScope(in RenderFrame frame)
  {
  }

  protected internal virtual void Dispose()
  {
  }

  void IRenderContext.OnBeginFrame(in RenderFrame frame)
  {
    OnBeginFrame(in frame);
  }

  void IRenderContext.OnEndFrame(in RenderFrame frame)
  {
    OnEndFrame(in frame);
  }

  void IRenderContext.OnBeginScope(in RenderFrame frame)
  {
    OnBeginScope(in frame);
  }

  void IRenderContext.OnEndScope(in RenderFrame frame)
  {
    OnEndScope(in frame);
  }

  void IDisposable.Dispose()
  {
    Dispose();
  }
}

/// <summary>
/// Helper methods for <see cref="IRenderContext"/>.
/// </summary>
public static class RenderContextExtensions
{
  /// <summary>
  /// Acquires a scoped reference to this <see cref="RenderContext"/>.
  /// </summary>
  public static RenderContextScope AcquireScope(this IRenderContext context, in RenderFrame frame)
  {
    return new RenderContextScope(context, in frame);
  }

  /// <summary>
  /// A scoped reference to a <see cref="RenderContext"/>.
  /// </summary>
  public readonly struct RenderContextScope : IDisposable
  {
    private readonly IRenderContext _context;
    private readonly RenderFrame _frame;

    public RenderContextScope(IRenderContext context, in RenderFrame frame)
    {
      _frame = frame;
      _context = context;
      _context.OnBeginScope(in frame);
    }

    public void Dispose()
    {
      _context.OnEndScope(in _frame);
    }
  }
}
