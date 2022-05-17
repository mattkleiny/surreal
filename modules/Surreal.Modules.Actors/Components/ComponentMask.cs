namespace Surreal.Components;

/// <summary>Describes a union of component types for use in component queries.</summary>
public readonly record struct ComponentMask
{
  public static ComponentMask Empty => default;

  public static ComponentMask Of<T1>()
    => new(ComponentType.GetBit<T1>());

  public static ComponentMask Of<T1, T2>()
    => new(ComponentType.GetBit<T1>(), ComponentType.GetBit<T2>());

  public static ComponentMask Of<T1, T2, T3>()
    => new(ComponentType.GetBit<T1>(), ComponentType.GetBit<T2>(), ComponentType.GetBit<T3>());

  public static ComponentMask Of<T1, T2, T3, T4>()
    => new(ComponentType.GetBit<T1>(), ComponentType.GetBit<T2>(), ComponentType.GetBit<T3>(), ComponentType.GetBit<T4>());

  private readonly BigInteger mask;

  private ComponentMask(params BigInteger[] bits)
  {
    mask = 0;

    foreach (var bit in bits)
    {
      mask |= bit;
    }
  }

  internal bool Contains<T>()
  {
    return Contains(ComponentType.GetBit<T>());
  }

  internal bool Contains(ComponentType type)
  {
    return Contains(type.Bit);
  }

  internal bool Contains(BigInteger bit)
  {
    return (bit & mask) == bit;
  }

  public override string ToString()
  {
    if (mask <= 0) { return "Empty Aspect"; }

    var labels = new StringBuilder();

    foreach (var (type, _, _) in ComponentType.ForMask(mask))
    {
      if (labels.Length > 0)
      {
        labels.Append(" and ");
      }

      labels.Append(type.Name);
    }

    return $"Aspect of <{labels}>";
  }

  public bool Equals(ComponentMask other)
  {
    return mask == other.mask;
  }

  public override int GetHashCode()
  {
    return mask.GetHashCode();
  }
}
