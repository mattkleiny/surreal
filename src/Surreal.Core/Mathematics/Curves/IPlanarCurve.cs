using System.Numerics;
using JetBrains.Annotations;

namespace Surreal.Mathematics.Curves {
  public interface IPlanarCurve {
    [Pure] Vector2 SampleAt(Normal t);
    [Pure] Vector2 SampleDerivativeAt(Normal t);
  }
}