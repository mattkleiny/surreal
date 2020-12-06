using Surreal.Framework.Scenes.Entities.Components;

namespace Surreal.Framework.Scenes.Entities.Aspects {
  public sealed class AspectBuilder {
    private readonly ComponentMask mask = new();

    public AspectBuilder Include<TComponent>()
        where TComponent : IComponent {
      mask.Include<TComponent>();
      return this;
    }

    public AspectBuilder Exclude<TComponent>()
        where TComponent : IComponent {
      mask.Exclude<TComponent>();
      return this;
    }

    public Aspect Build() => new(mask);
  }
}