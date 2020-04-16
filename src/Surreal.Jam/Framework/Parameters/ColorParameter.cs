using System.Diagnostics;
using Surreal.Graphics;

namespace Surreal.Framework.Parameters
{
  [DebuggerDisplay("Color <{Value}>")]
  public sealed class ColorParameter : Parameter<Color>
  {
    public ColorParameter(Color value)
      : base(value)
    {
    }
  }
}
