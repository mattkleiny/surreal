using System.Diagnostics;
using System.Numerics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Vector3 {Value}")]
  public sealed class Vector3Parameter : Parameter<Vector3> {
    public Vector3Parameter(Vector3 value)
        : base(value) {
    }
  }
}