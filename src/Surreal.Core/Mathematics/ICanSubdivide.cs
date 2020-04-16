using System.Collections.Generic;

namespace Surreal.Mathematics
{
  public interface ICanSubdivide<out T>
  {
    IEnumerable<T> Subdivide(int regionWidth, int regionHeight);
  }
}
