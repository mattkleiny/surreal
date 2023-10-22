﻿using Surreal.Diagnostics.Logging;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a manager for <see cref="IRenderContext"/>s.
/// </summary>
public interface IRenderContextManager
{
  /// <summary>
  /// Adds a <see cref="IRenderContext"/> to the manager.
  /// </summary>
  void AddContext<T>(T context)
    where T : IRenderContext;

  /// <summary>
  /// Notifies the manager that a new frame is beginning.
  /// </summary>
  void OnBeginFrame(in RenderFrame frame);

  /// <summary>
  /// Notifies the manager that the current frame is ending.
  /// </summary>
  void OnEndFrame(in RenderFrame frame);

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager;
  /// if successful, returns a scoped reference to the context.
  /// <para/>
  /// The scoped reference will call <see cref="IRenderContext.OnBeginScope"/>
  /// and <see cref="IRenderContext.OnEndScope"/> when it is created and disposed.
  /// </summary>
  bool TryAcquireContext<TContext>(in RenderFrame frame, out TContext result)
    where TContext : IRenderContext;

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager. If unsuccessful, throws an exception.
  /// </summary>
  TContext AcquireContext<TContext>(in RenderFrame frame)
    where TContext : IRenderContext;
}

/// <summary>
/// The default <see cref="IRenderContextManager"/> implementation.
/// </summary>
public sealed class RenderContextManager(IGraphicsBackend backend) : IRenderContextManager, IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<RenderContextManager>();

  private readonly Dictionary<Type, IRenderContext> _contexts = new();

  /// <inheritdoc/>
  public void AddContext<T>(T context)
    where T : IRenderContext
  {
    Log.Trace($"Registering render context {typeof(T)}");

    _contexts.Add(typeof(T), context);
  }

  /// <inheritdoc/>
  public void OnBeginFrame(in RenderFrame frame)
  {
    foreach (var context in _contexts.Values)
    {
      context.OnBeginFrame(in frame);
    }
  }

  /// <inheritdoc/>
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
  /// The scoped reference will call <see cref="IRenderContext.OnBeginScope"/>
  /// and <see cref="IRenderContext.OnEndScope"/> when it is created and disposed.
  /// </summary>
  public bool TryAcquireContext<TContext>(in RenderFrame frame, out TContext result)
    where TContext : IRenderContext
  {
    if (_contexts.TryGetValue(typeof(TContext), out var context))
    {
      result = (TContext)context;
      return true;
    }

    result = default;
    return false;
  }

  /// <summary>
  /// Attempts to acquire a context of the given type from the manager. If unsuccessful, throws an exception.
  /// </summary>
  public TContext AcquireContext<TContext>(in RenderFrame frame)
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
      if (context is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }

    _contexts.Clear();
  }
}
