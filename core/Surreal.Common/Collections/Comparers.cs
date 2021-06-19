using System.Collections.Generic;

namespace Surreal.Collections
{
  public static class Comparers
  {
    public static Comparer<int> IntAscending { get; } = Comparer<int>.Create((a, b) =>
    {
      if (a > b) return 1;
      if (a < b) return -1;

      return 0;
    });

    public static Comparer<int> IntDescending { get; } = Comparer<int>.Create((a, b) =>
    {
      if (b > a) return 1;
      if (b < a) return -1;

      return 0;
    });

    public static Comparer<float> FloatAscending { get; } = Comparer<float>.Create((a, b) =>
    {
      if (a > b) return 1;
      if (a < b) return -1;

      return 0;
    });

    public static Comparer<float> FloatDescending { get; } = Comparer<float>.Create((a, b) =>
    {
      if (b > a) return 1;
      if (b < a) return -1;

      return 0;
    });
  }
}