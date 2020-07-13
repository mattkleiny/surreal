using System.Diagnostics;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.PathFinding {
  [DebuggerDisplay("Path with {Length} steps")]
  public readonly struct Path {
    public Path(Vector2I[] steps) {
      Steps = steps;
    }

    public Vector2I[] Steps   { get; }
    public Vector2I   Start   => Steps?[0]     ?? Vector2I.Zero;
    public Vector2I   Goal    => Steps?[^1]    ?? Vector2I.Zero;
    public int        Length  => Steps?.Length ?? 0;
    public bool       IsValid => Length > 0;
  }
}