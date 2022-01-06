namespace Surreal.Components;

/// <summary>A mask for a component, with a shared unique identifier to represent it.</summary>
public readonly record struct ComponentType(Type Type, int Id, BigInteger Bit)
{
  private static readonly Dictionary<Type, ComponentType> Metadata = new();

  private static int        nextId  = 0;
  private static BigInteger nextBit = 1;

  public static int        GetId<T>()  => GetOrCreate<T>().Id;
  public static BigInteger GetBit<T>() => GetOrCreate<T>().Bit;

  public static ComponentType GetOrCreate<T>()
  {
    return GetOrCreate(typeof(T));
  }

  public static ComponentType GetOrCreate(Type type)
  {
    if (!Metadata.TryGetValue(type, out var result))
    {
      var id  = Interlocked.Increment(ref nextId);
      var bit = nextBit <<= 1; // TODO: interlocked increment?

      Metadata[type] = result = new ComponentType(type, id, bit);
    }

    return result;
  }

  /// <summary>Creates an enumerable from a <see cref="BigInteger"/>  which holds type bits.</summary>
  internal static IEnumerable<ComponentType> ForMask(BigInteger bits)
  {
    foreach (var type in Metadata.Values)
    {
      if ((type.Bit & bits) != 0)
      {
        yield return type;
      }
    }
  }
}
