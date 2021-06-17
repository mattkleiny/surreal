using System;
using System.Diagnostics;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding {
  [DebuggerDisplay("Path with {Length} steps")]
  public readonly ref struct Path {
    private readonly Span<Vector2I> steps;
    private readonly IDisposable?   disposable;

    public Path(Span<Vector2I> steps, IDisposable? disposable = null) {
      this.steps      = steps;
      this.disposable = disposable;
    }

    public bool IsValid => !steps.IsEmpty;
    public bool IsEmpty => steps.IsEmpty;
    public int  Length  => steps.Length;

    public Vector2I this[Index index] => steps[index];
    public Span<Vector2I> this[Range range] => steps[range];

    public void Dispose() => disposable?.Dispose();
  }
}