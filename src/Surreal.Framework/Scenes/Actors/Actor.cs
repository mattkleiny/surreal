using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Actors
{
  public abstract class Actor : IDisposable, IEnumerable<Actor>
  {
    private Matrix4x4 modelToWorld;
    private Matrix4x4 worldToModel;

    public Actor?          Parent    { get; set; }
    public ActorCollection Children  { get; }
    public bool            IsEnabled { get; set; } = true;
    public bool            IsVisible { get; set; } = true;

    public List<IActorComponent> Components { get; } = new List<IActorComponent>();

    public ref readonly Matrix4x4 ModelToWorld => ref modelToWorld;
    public ref readonly Matrix4x4 WorldToModel => ref worldToModel;

    protected Actor()
    {
      Children = new ActorCollection(this);
    }

    public void Add(Actor actor)    => Children.Add(actor);
    public void Remove(Actor actor) => Children.Remove(actor);

    protected void RecomputeTransform()
    {
      ComputeModelToWorld(out modelToWorld);

      if (Parent != null)
      {
        modelToWorld *= Parent.modelToWorld;
      }

      Matrix4x4.Invert(modelToWorld, out worldToModel);
    }

    protected abstract void ComputeModelToWorld(out Matrix4x4 modelToWorld);

    public virtual void Begin()
    {
      RecomputeTransform();

      for (var i = 0; i < Components.Count; i++)
      {
        Components[i].Begin();
      }

      for (var i = 0; i < Children.Count; i++)
      {
        var child = Children[i];
        if (child.IsEnabled)
        {
          child.Begin();
        }
      }
    }

    public virtual void Input(DeltaTime deltaTime)
    {
      for (var i = 0; i < Components.Count; i++)
      {
        Components[i].Input(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++)
      {
        var child = Children[i];
        if (child.IsEnabled)
        {
          child.Input(deltaTime);
        }
      }
    }

    public virtual void Update(DeltaTime deltaTime)
    {
      for (var i = 0; i < Components.Count; i++)
      {
        Components[i].Update(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++)
      {
        var child = Children[i];
        if (child.IsEnabled)
        {
          child.Update(deltaTime);
        }
      }
    }

    public virtual void Draw(DeltaTime deltaTime)
    {
      for (var i = 0; i < Components.Count; i++)
      {
        Components[i].Draw(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++)
      {
        var child = Children[i];
        if (child.IsEnabled && child.IsVisible)
        {
          child.Draw(deltaTime);
        }
      }
    }

    public virtual void End()
    {
      for (var i = 0; i < Components.Count; i++)
      {
        Components[i].End();
      }

      for (var i = 0; i < Children.Count; i++)
      {
        var child = Children[i];
        if (child.IsEnabled)
        {
          child.End();
        }
      }
    }

    public virtual void Dispose()
    {
      for (var i = 0; i < Children.Count; i++)
      {
        Children[i].Dispose();
      }
    }

    public IEnumerable<Actor> GetAllChildren()
    {
      yield return this;

      foreach (var child in Children)
      foreach (var node in child.GetAllChildren())
      {
        yield return node;
      }
    }

    public List<Actor>.Enumerator         GetEnumerator() => Children.GetEnumerator();
    IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
  }
}
