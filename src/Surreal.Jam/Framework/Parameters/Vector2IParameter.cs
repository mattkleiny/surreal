using System.Diagnostics;
using Surreal.Mathematics.Linear;

namespace Surreal.Framework.Parameters
{
  [DebuggerDisplay("Vector2I {Value}")]
  public sealed class Vector2IParameter : Parameter<Vector2I>
  {
    public Vector2IParameter(Vector2I value)
      : base(value)
    {
    }
  }
}
