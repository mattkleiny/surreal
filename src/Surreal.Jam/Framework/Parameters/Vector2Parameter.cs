using System.Diagnostics;
using System.Numerics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Vector2 {Value}")]
  public sealed class Vector2Parameter : Parameter<Vector2> {
    public Vector2Parameter(Vector2 value)
        : base(value) {
    }
  }
}