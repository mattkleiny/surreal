using System.Collections;
using System.Collections.Generic;

namespace Surreal.UI.Controls {
  public sealed class ControlCollection : IEnumerable<Control> {
    private readonly List<Control> entries = new List<Control>();
    private readonly Control?      parent;

    public ControlCollection(Control? parent) {
      this.parent = parent;
    }

    public int Count => entries.Count;

    public Control this[int index] => entries[index];

    public void Add(Control control) {
      control.Parent = parent;
      entries.Add(control);
    }

    public void Remove(Control control) {
      control.Parent = null;
      entries.Remove(control);
    }

    public List<Control>.Enumerator           GetEnumerator() => entries.GetEnumerator();
    IEnumerator<Control> IEnumerable<Control>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
  }
}