using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Graphics.Sprites;
using Surreal.Mathematics.Linear;
using Surreal.Timing;

namespace Surreal.UI.Controls {
  public abstract class Control : IDisposable, IEnumerable<Control> {
    private Rectangle layout;

    public Control?          Parent    { get; set; } = null;
    public bool              IsEnabled { get; set; } = true;
    public bool              IsVisible { get; set; } = true;
    public ControlCollection Children  { get; }

    public ref readonly Rectangle Layout => ref layout;

    public Control() => Children = new ControlCollection(this);

    public void Add(Control control)    => Children.Add(control);
    public void Remove(Control control) => Children.Remove(control);

    protected abstract Rectangle ComputeLayout();

    public virtual void Begin() {
      layout = ComputeLayout();

      for (var i = 0; i < Children.Count; i++) {
        var control = Children[i];
        if (control.IsEnabled) {
          control.Begin();
        }
      }
    }

    public virtual void Input(DeltaTime deltaTime) {
      for (var i = 0; i < Children.Count; i++) {
        var control = Children[i];
        if (control.IsEnabled) {
          control.Input(deltaTime);
        }
      }
    }

    public virtual void Update(DeltaTime deltaTime) {
      for (var i = 0; i < Children.Count; i++) {
        var control = Children[i];
        if (control.IsEnabled) {
          control.Update(deltaTime);
        }
      }
    }

    public virtual void Draw(DeltaTime deltaTime, SpriteBatch batch) {
      for (var i = 0; i < Children.Count; i++) {
        var control = Children[i];
        if (control.IsEnabled && control.IsVisible) {
          control.Draw(deltaTime, batch);
        }
      }
    }

    public virtual void End() {
      for (var i = 0; i < Children.Count; i++) {
        var control = Children[i];
        if (control.IsEnabled) {
          control.End();
        }
      }
    }

    public virtual void Dispose() {
      for (var i = 0; i < Children.Count; i++) {
        Children[i].Dispose();
      }
    }

    public List<Control>.Enumerator           GetEnumerator() => Children.GetEnumerator();
    IEnumerator<Control> IEnumerable<Control>.GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
  }
}