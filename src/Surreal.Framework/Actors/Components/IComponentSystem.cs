using System;
using System.Collections;
using System.Collections.Generic;

namespace Surreal.Framework.Actors.Components
{
  public interface IComponentSystem
  {
    void OnInput(GameTime time);
    void OnUpdate(GameTime time);
    void OnDraw(GameTime time);
  }

  public interface IComponentContext
  {
    IComponentStorage<T> GetStorage<T>();
    AspectEnumerator     GetEnumerator(Aspect aspect);
  }

  public abstract class ComponentSystem : IComponentSystem
  {
    public IComponentContext Context { get; }

    protected ComponentSystem(IComponentContext context)
    {
      Context = context;
    }

    public virtual void OnInput(GameTime time)
    {
    }

    public virtual void OnUpdate(GameTime time)
    {
    }

    public virtual void OnDraw(GameTime time)
    {
    }
  }

  public abstract class IteratingSystem : ComponentSystem
  {
    public Aspect Aspect { get; }

    protected IteratingSystem(IComponentContext context, Aspect aspect)
        : base(context)
    {
      Aspect = aspect;
    }

    public sealed override void OnInput(GameTime time)
    {
      OnBeginInput(time);

      foreach (var actor in Context.GetEnumerator(Aspect))
      {
        OnInput(time, actor);
      }

      OnEndInput(time);
    }

    protected virtual void OnBeginInput(GameTime time)
    {
    }

    protected virtual void OnInput(GameTime time, ActorId actor)
    {
    }

    protected virtual void OnEndInput(GameTime time)
    {
    }

    public sealed override void OnUpdate(GameTime time)
    {
      OnBeginUpdate(time);

      foreach (var actor in Context.GetEnumerator(Aspect))
      {
        OnUpdate(time, actor);
      }

      OnEndUpdate(time);
    }

    protected virtual void OnBeginUpdate(GameTime time)
    {
    }

    protected virtual void OnUpdate(GameTime time, ActorId actor)
    {
    }

    protected virtual void OnEndUpdate(GameTime time)
    {
    }

    public sealed override void OnDraw(GameTime time)
    {
      OnBeginDraw(time);

      foreach (var actor in Context.GetEnumerator(Aspect))
      {
        OnDraw(time, actor);
      }

      OnEndDraw(time);
    }

    protected virtual void OnBeginDraw(GameTime time)
    {
    }

    protected virtual void OnDraw(GameTime time, ActorId actor)
    {
    }

    protected virtual void OnEndDraw(GameTime time)
    {
    }
  }

  public readonly struct Aspect : IEquatable<Aspect>
  {
    public static Aspect Of<T1>()             => new(HashCode.Combine(typeof(T1)));
    public static Aspect Of<T1, T2>()         => new(HashCode.Combine(typeof(T1), typeof(T2)));
    public static Aspect Of<T1, T2, T3>()     => new(HashCode.Combine(typeof(T1), typeof(T2), typeof(T3)));
    public static Aspect Of<T1, T2, T3, T4>() => new(HashCode.Combine(typeof(T1), typeof(T2), typeof(T3), typeof(T4)));

    private readonly int hash;

    private Aspect(int hash) => this.hash = hash;

    public          bool Equals(Aspect other) => hash == other.hash;
    public override bool Equals(object? obj)  => obj is Aspect other && Equals(other);

    public override int GetHashCode() => hash;

    public static bool operator ==(Aspect left, Aspect right) => left.Equals(right);
    public static bool operator !=(Aspect left, Aspect right) => !left.Equals(right);
  }

  public struct AspectEnumerator : IEnumerator<ActorId>, IEnumerable<ActorId>
  {
    private readonly IEnumerator<ActorId> enumerator;

    public AspectEnumerator(IEnumerator<ActorId> enumerator)
    {
      this.enumerator = enumerator;
    }

    public ActorId     Current => enumerator.Current;
    object IEnumerator.Current => Current;

    public bool MoveNext() => enumerator.MoveNext();
    public void Reset()    => enumerator.Reset();
    public void Dispose()  => enumerator.Dispose();

    public AspectEnumerator                   GetEnumerator() => this;
    IEnumerator<ActorId> IEnumerable<ActorId>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
  }
}