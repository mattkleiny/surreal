using Surreal.Components;

namespace Surreal.Aspects;

/// <summary>Describes a union of component types for use in component queries.</summary>
public readonly record struct Aspect(ComponentMask Includes = default, ComponentMask Excludes = default, ComponentMask OneOf = default)
{
  public Aspect With<T>() => this with { Includes = Includes.Include(ComponentType.For<T>()) };
  public Aspect Without<T>() => this with { Excludes = Excludes.Exclude(ComponentType.For<T>()) };
  public Aspect WithOneOf<T>() => this with { OneOf = OneOf.Exclude(ComponentType.For<T>()) };

  public bool IsInterestedIn(ComponentMask mask)
  {
    if (!Includes.IsEmpty && !mask.ContainsAll(Includes))
    {
      return false;
    }

    if (!Excludes.IsEmpty && mask.ContainsAny(Excludes))
    {
      return false;
    }

    if (!OneOf.IsEmpty && mask.ContainsAny(OneOf))
    {
      return false;
    }

    return true;
  }
}
