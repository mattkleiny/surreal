using System.Diagnostics;
using Surreal.Mathematics;

namespace Surreal.Framework.Parameters {
  [DebuggerDisplay("Angle <{Value}>")]
  public sealed class AngleParameter : Parameter<Angle> {
    public AngleParameter(Angle value)
        : base(value) {
    }
  }
}