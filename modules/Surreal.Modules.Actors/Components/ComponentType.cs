namespace Surreal.Components;

/// <summary>A type of a component, with a shared unique identifier to represent it.</summary>
public readonly record struct ComponentType(Type Type, int Id, ComponentMask Mask)
{
  private static readonly ConcurrentDictionary<Type, ComponentType> Metadata = new();
  private static readonly object BitLocker = new();

  private static int nextId = 0;
  private static BigInteger nextBit = 1;

  public static ComponentType For<T>()
  {
    return For(typeof(T));
  }

  public static ComponentType For(Type type)
  {
    static ComponentType CreateComponent(Type type)
    {
      lock (BitLocker)
      {
        var id = Interlocked.Increment(ref nextId);
        var bit = nextBit <<= 1;
        var mask = new ComponentMask(bit);

        return new ComponentType(type, id, mask);
      }
    }

    return Metadata.GetOrAdd(type, CreateComponent);
  }

  /// <summary>Determines all <see cref="ComponentType"/>s that match the given mask.</summary>
  internal static IEnumerable<ComponentType> ForMask(ComponentMask mask)
  {
    foreach (var type in Metadata.Values)
    {
      if (mask.ContainsAll(type.Mask))
      {
        yield return type;
      }
    }
  }
}
