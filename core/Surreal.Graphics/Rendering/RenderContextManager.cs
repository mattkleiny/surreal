using Surreal.Diagnostics.Logging;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a manager for <see cref="RenderContext"/>s.
/// </summary>
public interface IRenderContextManager
{
  /// <summary>
  /// Attempts to acquire a context of the given type from the manager;
  /// if successful, returns a scoped reference to the context.
  /// </summary>
  bool TryGetContext<TContext>(in RenderFrame frame, out TContext result)
    where TContext : RenderContext;
}

/// <summary>
/// The default <see cref="IRenderContextManager"/> implementation.
/// </summary>
public sealed class RenderContextManager : IRenderContextManager, IEnumerable<RenderContext>, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<RenderContextManager>();

  private readonly Dictionary<Type, RenderContext> _contexts = new();

  /// <summary>
  /// Adds a <see cref="RenderContext"/> to the manager.
  /// </summary>
  public void Add(RenderContext context)
  {
    var type = context.GetType();

    Log.Trace($"Registering {type}");

    _contexts.Add(type, context);
  }

  /// <summary>
  /// Notifies the manager that a new frame is rendering.
  /// </summary>
  public void OnBeginFrame(in RenderFrame frame)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnBeginFrame(in frame);
    }
  }

  /// <summary>
  /// Notifies the manager that a frame is finishing.
  /// </summary>
  public void OnEndFrame(in RenderFrame frame)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnEndFrame(in frame);
    }
  }

  /// <summary>
  /// Notifies the manager that a viewport is rendering.
  /// </summary>
  public void OnBeginViewport(in RenderFrame frame, IRenderViewport viewport)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnBeginViewport(in frame, viewport);
    }
  }

  /// <summary>
  /// Notifies the manager that a viewport is rendering.
  /// </summary>
  public void OnEndViewport(in RenderFrame frame, IRenderViewport viewport)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnEndViewport(in frame, viewport);
    }
  }

  /// <summary>
  /// Notifies the manager that a pass is rendering.
  /// </summary>
  public void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnBeginPass(in frame, viewport);
    }
  }

  /// <summary>
  /// Notifies the manager that a pass is rendering.
  /// </summary>
  public void OnEndPass(in RenderFrame frame, IRenderViewport viewport)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnEndPass(in frame, viewport);
    }
  }

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager;
  /// if successful, returns a scoped reference to the context.
  /// </summary>
  public bool TryGetContext<TContext>(in RenderFrame frame, out TContext result)
    where TContext : RenderContext
  {
    if (_contexts.TryGetValue(typeof(TContext), out var context))
    {
      result = (TContext)context;
      return true;
    }

    result = default!;
    return false;
  }

  /// <summary>
  /// Removes all contexts from the manager.
  /// </summary>
  public void Clear()
  {
    foreach (var context in _contexts.Values)
    {
      if (context is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    _contexts.Clear();
  }

  public void Dispose()
  {
    Clear();
  }

  public Dictionary<Type, RenderContext>.ValueCollection.Enumerator GetEnumerator()
  {
    return _contexts.Values.GetEnumerator();
  }

  IEnumerator<RenderContext> IEnumerable<RenderContext>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
