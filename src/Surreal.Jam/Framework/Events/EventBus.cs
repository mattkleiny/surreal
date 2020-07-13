using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;

namespace Surreal.Framework.Events {
  public sealed class EventBus : IEventBus {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<EventBus>();

    private readonly ConcurrentQueue<object> events        = new ConcurrentQueue<object>();
    private readonly Queue<object>           executionPool = new Queue<object>();

    private readonly IEventDispatcher       dispatcher;
    private readonly IEventListenerResolver resolver;

    public EventBus()
        : this(new CachingEventDispatcher(), new AttributedEventListenerResolver()) {
    }

    public EventBus(IEventDispatcher dispatcher, IEventListenerResolver resolver) {
      this.dispatcher = dispatcher;
      this.resolver   = resolver;
    }

    public void RegisterListeners(object target) {
      dispatcher.Register(resolver.Resolve(target));
    }

    public void Publish(object @event) {
      events.Enqueue(@event);
    }

    public void Dispatch() {
      // transfer scheduled tasks into a local pool for execution
      while (events.TryDequeue(out var @event)) {
        executionPool.Enqueue(@event);
      }

      while (executionPool.TryDequeue(out var @event)) {
        using (Profiler.Track(nameof(Dispatch))) {
          dispatcher.Dispatch(@event);
        }
      }
    }

    public interface IEventListener {
      bool CanHandle(object @event);
      void Receive(object @event);
    }

    public interface IEventDispatcher {
      void Register(IEnumerable<IEventListener> listeners);
      void Dispatch(object @event);
    }

    public sealed class SimpleEventDispatcher : IEventDispatcher {
      private readonly List<IEventListener> listeners = new List<IEventListener>();

      public void Register(IEnumerable<IEventListener> listeners) => this.listeners.AddRange(listeners);

      public void Dispatch(object @event) {
        for (var i = 0; i < listeners.Count; i++) {
          var listener = listeners[i];

          if (listener.CanHandle(@event)) {
            listener.Receive(@event);
          }
        }
      }
    }

    public sealed class CachingEventDispatcher : IEventDispatcher {
      private readonly List<IEventListener>                  listeners       = new List<IEventListener>();
      private readonly MultiDictionary<Type, IEventListener> listenersByType = new MultiDictionary<Type, IEventListener>();

      public void Register(IEnumerable<IEventListener> listeners) {
        this.listeners.AddRange(listeners);
        listenersByType.Clear();
      }

      public void Dispatch(object @event) {
        var eventType = @event.GetType();

        // cache listeners for each encountered event type the first time those events are encountere
        if (!listenersByType.TryGetValues(eventType, out var targets)) {
          foreach (var listener in listeners) {
            if (!listener.CanHandle(@event)) continue;
            listenersByType.Add(eventType, listener);
          }

          targets = listenersByType[eventType];
        }

        // dispatch to each target listener
        for (var i = 0; i < targets.Count; i++) {
          targets[i].Receive(@event);
        }
      }
    }

    public interface IEventListenerResolver {
      IEnumerable<IEventListener> Resolve(object target);
    }

    public sealed class AttributedEventListenerResolver : IEventListenerResolver {
      public IEnumerable<IEventListener> Resolve(object target) => target
          .GetType().GetMethods()
          .Where(method => method.GetCustomAttribute<SubscribeAttribute>() != null)
          .Where(method => method.GetParameters().Length                   == 1)
          .Select(method => new WeakMethodEventListener(target, method))
          .ToArray();

      private sealed class WeakMethodEventListener : IEventListener {
        private readonly WeakReference reference;
        private readonly Type          parameterType;
        private readonly Delegate      callback;

        public WeakMethodEventListener(object target, MethodInfo method) {
          // create a delegate for invoking the method via reflection
          reference     = new WeakReference(target);
          parameterType = method.GetParameters()[0].ParameterType; // we've already verified there is only 1 parameter
          callback = method.CreateDelegate(typeof(Action<,>)
              .MakeGenericType(target.GetType(), parameterType));
        }

        public bool CanHandle(object @event) => @event.GetType().IsAssignableFrom(parameterType);

        public void Receive(object @event) {
          if (!reference.IsAlive) return;

          callback.DynamicInvoke(reference.Target, @event);
        }
      }
    }
  }
}