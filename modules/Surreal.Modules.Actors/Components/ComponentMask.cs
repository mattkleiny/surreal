namespace Surreal.Components;

/// <summary>A mask for a component, with a shared unique identifier to represent it.</summary>
public readonly record struct ComponentMask(BigInteger Mask)
{
  public static ComponentMask Empty => default;
  public static ComponentMask For<T>() => ComponentType.For<T>().Mask;

  public bool IsEmpty => Mask == 0;

  public bool ContainsAll(ComponentMask other) => (other.Mask & Mask) == other.Mask;
  public bool ContainsAny(ComponentMask other) => (other.Mask & Mask) != 0;

  public ComponentMask Include<T>() => Include(ComponentType.For<T>());
  public ComponentMask Include(ComponentType type) => this | type.Mask;

  public ComponentMask Exclude<T>() => Exclude(ComponentType.For<T>());
  public ComponentMask Exclude(ComponentType type) => this ^ type.Mask;

  public override string ToString()
  {
    if (IsEmpty) return "Empty Mask";

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

  public static ComponentMask operator |(ComponentMask left, ComponentMask right) => new(left.Mask | right.Mask);
  public static ComponentMask operator ^(ComponentMask left, ComponentMask right) => new(left.Mask ^ right.Mask);
}
