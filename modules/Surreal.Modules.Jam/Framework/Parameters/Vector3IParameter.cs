using System.Diagnostics;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Point3 {Value}")]
  public sealed class Vector3IParameter : Parameter<Vector3I> {
    public Vector3IParameter(Vector3I value)
        : base(value) {
    }
  }
}