using System.Diagnostics;
using System.Numerics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Vector4 {Value}")]
  public sealed class Vector4Parameter : Parameter<Vector4> {
    public Vector4Parameter(Vector4 value)
        : base(value) {
    }
  }
}