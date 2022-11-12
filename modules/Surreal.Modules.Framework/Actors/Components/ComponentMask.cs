namespace Surreal.Components;

/// <summary>A mask for a component, with a shared unique identifier to represent it.</summary>
public readonly record struct ComponentMask(BigInteger Mask)
{
  public static ComponentMask Empty => default;

  public bool IsEmpty => Mask == 0;

  public static ComponentMask For<T>()
  {
    return ComponentType.For<T>().Mask;
  }

  public bool ContainsAll(ComponentMask other)
  {
    return (other.Mask & Mask) == other.Mask;
  }

  public bool ContainsAny(ComponentMask other)
  {
    return (other.Mask & Mask) != 0;
  }

  public ComponentMask Include<T>()
  {
    return Include(ComponentType.For<T>());
  }

  public ComponentMask Include(ComponentType type)
  {
    return this | type.Mask;
  }

  public ComponentMask Exclude<T>()
  {
    return Exclude(ComponentType.For<T>());
  }

  public ComponentMask Exclude(ComponentType type)
  {
    return this ^ type.Mask;
  }

  public override string ToString()
  {
    if (IsEmpty)
    {
      return "Empty Mask";
    }

    var builder = new StringBuilder();

    foreach (var (type, _, _) in ComponentType.ForMask(this))
    {
      if (builder.Length > 0)
      {
        builder.Append(", ");
      }

      builder.Append(type.Name);
    }

    return builder.ToString();
  }

  public static ComponentMask operator |(ComponentMask left, ComponentMask right)
  {
    return new ComponentMask(left.Mask | right.Mask);
  }

  public static ComponentMask operator ^(ComponentMask left, ComponentMask right)
  {
    return new ComponentMask(left.Mask ^ right.Mask);
  }
}


