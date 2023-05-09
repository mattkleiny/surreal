using Surreal.Diagnostics.Logging;
using Surreal.Resources;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A manager for <see cref="IRenderContext"/>s.
/// </summary>
public sealed class RenderContextManager : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<RenderContextManager>();

  private readonly Dictionary<Type, IRenderContext> _contexts = new();
  private readonly IGraphicsServer _graphicsServer;
  private readonly IResourceManager _resourceManager;

  public RenderContextManager(IGraphicsServer graphicsServer, IResourceManager resourceManager)
  {
    _graphicsServer = graphicsServer;
    _resourceManager = resourceManager;
  }

  public void AddContext<T>(T context)
    where T : IRenderContext
  {
    Log.Trace($"Registering render context {typeof(T)}");

    _contexts.Add(typeof(T), context);
  }

  public async Task AddContextAsync(IRenderContextDescriptor descriptor, CancellationToken cancellationToken = default)
  {
    Log.Trace($"Registering render context descriptor {descriptor.GetType()}");

    var context = await descriptor.BuildContextAsync(_graphicsServer, _resourceManager, cancellationToken);

    _contexts.Add(context.GetType(), context);
  }

  public void OnBeginFrame(in RenderFrame frame)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnBeginFrame(in frame);
    }
  }

  public void OnEndFrame(in RenderFrame frame)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnEndFrame(in frame);
    }
  }

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager;
  /// if successful, returns a scoped reference to the context.
  /// <para/>
  /// The scoped reference will call <see cref="IRenderContext.OnBeginUse"/>
  /// and <see cref="IRenderContext.OnEndUse"/> when it is created and disposed.
  /// </summary>
  public bool TryAcquireContext<TContext>(in RenderFrame frame, out RenderContextScope<TContext> result)
    where TContext : IRenderContext
  {
    if (_contexts.TryGetValue(typeof(TContext), out var context))
    {
      result = new RenderContextScope<TContext>(frame, (TContext)context);
      return true;
    }

    result = default;
    return false;
  }

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager. If unsuccessful, throws an exception.
  /// </summary>
  public RenderContextScope<TContext> AcquireContext<TContext>(in RenderFrame frame)
    where TContext : IRenderContext
  {
    if (!TryAcquireContext<TContext>(in frame, out var result))
    {
      throw new InvalidOperationException($"No context of type {typeof(TContext)} is available.");
    }

    return result;
  }

  public void Dispose()
  {
    foreach (var context in _contexts.Values)
    {
      context.Dispose();
    }

    _contexts.Clear();
  }

  /// <summary>
  /// A managed scoped reference to a <see cref="IRenderContext"/>.
  /// </summary>
  public readonly struct RenderContextScope<TContext> : IDisposable
    where TContext : IRenderContext
  {
    private readonly RenderFrame _frame;

    public TContext Context { get; }

    public RenderContextScope(RenderFrame frame, TContext context)
    {
      _frame = frame;

      Context = context;
      Context.OnBeginUse(in _frame);
    }

    public void Dispose()
    {
      Context.OnEndUse(in _frame);
    }
  }
}
