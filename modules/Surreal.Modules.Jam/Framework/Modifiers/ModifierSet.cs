using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Collections;

namespace Surreal.Framework.Modifiers {
  public abstract class ModifierSet<T> : IEnumerable<Modifier<T>> {
    private static readonly Comparison<Modifier<T>> ModifierOrder = (a, b) => {
      if (a.Order < b.Order) return -1;
      if (a.Order > b.Order) return 1;

      return 0;
    };

    private readonly Bag<Modifier<T>> modifiers = new();
    private          bool             isDirty;

    protected abstract T Calculate(T baseValue, ReadOnlySpan<Modifier<T>> modifiers);

    public void MarkAsDirty() {
      isDirty = true;
    }

    public T Apply(in T input) {
      if (isDirty) {
        modifiers.Sort(ModifierOrder);
        isDirty = false;
      }

      return Calculate(input, modifiers.Span);
    }

    public void Add(Modifier<T> modifier) {
      modifiers.Add(modifier);
      MarkAsDirty();
    }

    public void Remove(Modifier<T> modifier) {
      modifiers.Remove(modifier);
      MarkAsDirty();
    }

    public void Clear() {
      modifiers.Clear();
      MarkAsDirty();
    }

    public Bag<Modifier<T>>.Enumerator                GetEnumerator() => modifiers.GetEnumerator();
    IEnumerator<Modifier<T>> IEnumerable<Modifier<T>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                          GetEnumerator() => GetEnumerator();
  }
}