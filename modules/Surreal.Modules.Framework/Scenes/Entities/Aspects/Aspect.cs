using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities.Aspects {
  public sealed class Aspect {
    public static Aspect Of<T1>()
        where T1 : IComponent => new AspectBuilder()
        .Include<T1>()
        .Build();

    public static Aspect Of<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent => new AspectBuilder()
        .Include<T1>()
        .Include<T2>()
        .Build();

    public static Aspect Of<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent => new AspectBuilder()
        .Include<T1>()
        .Include<T2>()
        .Include<T3>()
        .Build();

    private readonly ComponentMask mask;

    internal Aspect(ComponentMask mask) {
      this.mask = mask;
    }

    public bool InterestedIn<TComponent>()
        where TComponent : IComponent {
      return mask.InterestedIn<TComponent>();
    }

    private bool Equals(Aspect other) {
      return mask.Equals(other.mask);
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is Aspect other && Equals(other);
    }

    public override int GetHashCode() => mask.GetHashCode();
  }
}