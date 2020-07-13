using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Surreal.Mathematics;

namespace Surreal.Collections {
  public sealed class Atlas<T> : IReadOnlyList<T>
      where T : ICanSubdivide<T> {
    private readonly T[]                   regions;
    private readonly Dictionary<string, T> regionsByName;

    public static Atlas<T> Create(T source, string nameTemplate, int regionWidth, int regionHeight) {
      Debug.Assert(regionWidth  > 0, "regionWidth > 0");
      Debug.Assert(regionHeight > 0, "regionHeight > 0");

      var i = 0; // track offset into region array

      var regions = source.Subdivide(regionWidth, regionHeight).ToArray();
      var named   = regions.ToDictionary(region => nameTemplate + i++);

      return new Atlas<T>(regions, named);
    }

    private Atlas(T[] regions, Dictionary<string, T> regionsByName) {
      this.regions       = regions;
      this.regionsByName = regionsByName;
    }

    public bool ContainsRegion(string name) => regionsByName.ContainsKey(name);

    public int Count => regions.Length;

    public T this[int index] => regions[index];
    public T this[string name] => regionsByName[name];

    public IEnumerator<T>   GetEnumerator() => regions.Cast<T>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}