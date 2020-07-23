using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public abstract class Actor : IDisposable, IEnumerable<Actor> {
    private Matrix4x4 localToWorld     = Matrix4x4.Identity;
    private Matrix4x4 worldToLocal     = Matrix4x4.Identity;
    private bool      isEnabled        = true;
    private bool      isVisible        = true;
    private bool      isTransformDirty = true;

    public Actor?        Parent     { get; set; }
    public ActorList     Children   { get; }
    public ComponentList Components { get; }

    public ref readonly Matrix4x4 LocalToWorld => ref localToWorld;
    public ref readonly Matrix4x4 WorldToLocal => ref worldToLocal;

    public virtual bool IsEnabled {
      get => isEnabled && (Parent?.IsEnabled).GetValueOrDefault(true);
      set => isEnabled = value;
    }

    public virtual bool IsVisible {
      get => isVisible && (Parent?.IsVisible).GetValueOrDefault(true);
      set => isVisible = value;
    }

    protected Actor() {
      Children   = new ActorList(this);
      Components = new ComponentList(this);
    }

    public void Add(Actor actor)    => Children.Add(actor);
    public void Remove(Actor actor) => Children.Remove(actor);

    public void MarkTransformAsDirty() {
      isTransformDirty = true;
    }

    protected void RecomputeTransform() {
      ComputeModelToWorld(out localToWorld);

      if (Parent != null) {
        localToWorld *= Parent.localToWorld;
      }

      Matrix4x4.Invert(localToWorld, out worldToLocal);
      isTransformDirty = false;
    }

    protected abstract void ComputeModelToWorld(out Matrix4x4 modelToWorld);

    public virtual void Input(DeltaTime deltaTime) {
      for (var i = 0; i < Components.Count; i++) {
        Components[i].Input(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++) {
        var child = Children[i];
        if (child.IsEnabled) {
          child.Input(deltaTime);
        }
      }
    }

    public virtual void Update(DeltaTime deltaTime) {
      if (isTransformDirty) {
        RecomputeTransform();
      }

      for (var i = 0; i < Components.Count; i++) {
        Components[i].Update(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++) {
        var child = Children[i];
        if (child.IsEnabled) {
          child.Update(deltaTime);
        }
      }
    }

    public virtual void Draw(DeltaTime deltaTime) {
      for (var i = 0; i < Components.Count; i++) {
        Components[i].Draw(deltaTime);
      }

      for (var i = 0; i < Children.Count; i++) {
        var child = Children[i];
        if (child.IsEnabled && child.IsVisible) {
          child.Draw(deltaTime);
        }
      }
    }

    public virtual void Dispose() {
      for (var i = 0; i < Children.Count; i++) {
        Children[i].Dispose();
      }
    }

    public IEnumerable<Actor> GetChildrenRecursively() {
      yield return this;

      foreach (var child in Children)
      foreach (var node in child.GetChildrenRecursively()) {
        yield return node;
      }
    }

    public List<Actor>.Enumerator         GetEnumerator() => Children.GetEnumerator();
    IEnumerator<Actor> IEnumerable<Actor>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
  }
}