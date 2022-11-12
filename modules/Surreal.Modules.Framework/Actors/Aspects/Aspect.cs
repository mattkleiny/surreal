using Surreal.Actors.Components;

namespace Surreal.Actors.Aspects;

/// <summary>Describes a union of component types for use in component queries.</summary>
public readonly record struct Aspect(ComponentMask Includes = default, ComponentMask Excludes = default, ComponentMask OneOf = default)
{
  public Aspect With<T>()
  {
    return this with { Includes = Includes.Include(ComponentType.For<T>()) };
  }

  public Aspect Without<T>()
  {
    return this with { Excludes = Excludes.Exclude(ComponentType.For<T>()) };
  }

  public Aspect WithOneOf<T>()
  {
    return this with { OneOf = OneOf.Exclude(ComponentType.For<T>()) };
  }

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





